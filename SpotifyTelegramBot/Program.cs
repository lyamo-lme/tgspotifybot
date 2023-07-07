using SpotifyTelegramBot.Handles;
using Telegram.Bot;
using Telegram.Bot.Types;

var botClient = new TelegramBotClient("5865735507:AAHuLANbrOdS5956v8dMR9RarO9bx4Bq4Fc");

botClient.SetMyCommandsAsync(new List<BotCommand>
{
    new BotCommand { Command = "/start", Description = "Start bot in new chat" },
    new BotCommand { Command = "/guide", Description = "Guide of using" },
    new BotCommand { Command = "/download", Description = "Type /download {id}  to download ex: /dowload kjSrew" }
});

botClient.StartReceiving(UpdateHandler.UpdateHandlerMessage, ErrorHandler.ErrorHandlerMessage, null);
Console.ReadLine();