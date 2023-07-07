using SpotifyTelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SpotifyTelegramBot.Interfaces.DownloaderProvider;

public interface IMusicDownloader
{
    Task<DataFile> DownloadAudioAsync(SongDto dto);
    Task<List<DataFile>> DownloadAudiosAsync(SongDto[] dtos,ITelegramBotClient client, Update update, CancellationToken token);
    Task<byte[]> ToZipAsync(List<DataFile> data);
    ValueTask<List<DataFile>> Convert(List<DataFile> data, string outputFormat);
}