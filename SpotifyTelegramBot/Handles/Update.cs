using SpotifyTelegramBot.SpotifyDownloader;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SpotifyTelegramBot.Handles;

public class UpdateHandler
{
    public static List<Task> _tasks;
    public static void UpdateHandlerMessage(ITelegramBotClient client, Update update, CancellationToken token)
    {
        Task.Run(async () =>
        {
            try
            {
                var spotifyProvider = new SpotifyProvider();
                var tgWrapper = new TelegramMusicWrapper();
                var utube = new YouTubeMusicProvider();
                var chatId = update.Message.Chat.Id;
                switch (update.Message.Text)
                {
                    case "/start":
                    {
                        await client.SendTextMessageAsync(chatId,
                            $@"Hi,{update.Message.Chat.Username}, thx for registration.\n type /guide to get instructions to using of bot");
                        break;
                    }
                    case "/guide":
                    {
                        await client.SendTextMessageAsync(chatId, "not");
                        break;
                    }
                }

                if (update.Message.Text.Equals("/start") != true)
                {
                    var links = await spotifyProvider.GetLinks(update.Message.Text);
                    var tasks = links.Select(x => utube.DownloadAudioAsync(x));
                    await tgWrapper.DownloadAudiosAsync(tasks, client, update, token);
                }
            }
            catch
            {
                throw;
            }
        }).ConfigureAwait(false);
    }
}