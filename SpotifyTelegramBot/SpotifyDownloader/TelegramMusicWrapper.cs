using SpotifyTelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SpotifyTelegramBot.SpotifyDownloader;

public class TelegramMusicWrapper
{
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5, 5);

    public async Task DownloadAudiosAsync(IEnumerable<Task<DataFile>> tasks, ITelegramBotClient client, Update update,
        CancellationToken token)
    {
        var mesTasks = tasks.Select(async (audio) =>
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                var audioFile = await audio;
                await client.SendAudioAsync(
                    chatId: update.Message!.Chat.Id,
                    audio: InputFile.FromStream(audioFile.stream),
                    cancellationToken: token,
                    title: audioFile.title,
                    performer: audioFile.artist,
                    duration: audioFile.duration
                );
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
        await Task.WhenAll(mesTasks.ToList());
    }
}