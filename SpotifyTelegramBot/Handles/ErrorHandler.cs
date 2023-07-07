using Telegram.Bot;
using Telegram.Bot.Types;

public class ErrorHandler
{
    public async static void ErrorHandlerMessage(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        try
        {
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}