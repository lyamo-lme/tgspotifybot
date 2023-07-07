using System.IO.Compression;
using SpotifyTelegramBot.Interfaces.DownloaderProvider;
using SpotifyTelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace SpotifyTelegramBot.SpotifyDownloader;

public class YouTubeMusicProvider : IMusicDownloader
{
    private readonly YoutubeClient _youtube;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5, 5);


    public YouTubeMusicProvider()
    {
        _youtube = new YoutubeClient();
    }

    public async Task<DataFile> DownloadAudioAsync(SongDto dto)
    {
        try
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(dto.UrlUtube);
            var audioStreamInfo = streamManifest
                .GetAudioStreams()
                .Where(s => s.Container == Container.WebM)
                .GetWithHighestBitrate();

            using (var audio = await _youtube.Videos.Streams.GetAsync(audioStreamInfo))
            {
                return new DataFile
                {
                    typeFile = Container.WebM.ToString(),
                    stream = audio,
                    title = dto.Name,
                    duration = dto.DurationS,
                    artist = String.Join(" ", dto.Artists)
                };
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }


    public async Task<List<DataFile>> DownloadAudiosAsync(SongDto[] videoIds, ITelegramBotClient client, Update update,
        CancellationToken token)
    {
        var file = new List<DataFile>();

        var tasks = videoIds.Select(async videoId =>
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId.UrlUtube);
                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.WebM)
                    .GetWithHighestBitrate();

                await using (var audio = await _youtube.Videos.Streams.GetAsync(audioStreamInfo))
                {
                    await client.SendAudioAsync(
                        chatId: update.Message.Chat.Id,
                        audio: InputFile.FromStream(audio),
                        cancellationToken: token,
                        title: videoId.Name
                    );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        });
        await Task.WhenAll(tasks.ToList());

        return file;
    }

    public async Task<byte[]> ToZipAsync(List<DataFile> data)
    {
        using (var ms = new MemoryStream())
        {
            using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var attachment in data)
                {
                    var entry = zipArchive.CreateEntry($"{attachment.title}.{attachment.typeFile}",
                        CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    await using Stream stream = new MemoryStream(attachment.stream.ReadByte());
                    await stream.CopyToAsync(entryStream);
                }
            }

            return ms.ToArray();
        }
    }

    public async ValueTask<List<DataFile>> Convert(List<DataFile> data, string outputFormat)
    {
        var output = new List<DataFile>(data.Count());
        // /*need to write to collection and dispose not auto*/
        // foreach (var webmBytes in data)
        // {
        //     using (var webmStream = new MemoryStream(webmBytes.dataBytes))
        //     {
        //         using (var mp3Stream = new MemoryStream())
        //         {
        //             var ffMpeg = new FFMpegConverter();
        //             var convertTask = ffMpeg.ConvertLiveMedia(webmStream, webmBytes.TypeFile, mp3Stream, outputFormat,
        //                 new ConvertSettings());
        //             convertTask.Start();
        //             convertTask.Wait();
        //             output.Add(new DataFile()
        //             {
        //                 dataBytes = mp3Stream.ToArray(),
        //                 Name = webmBytes.Name
        //             });
        //         }
        //     }
        // }

        return output;
    }
}