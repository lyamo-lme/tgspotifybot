namespace SpotifyTelegramBot.Models;
/*many to many tg user with spotify song id*/
public class TgUserSpAuido
{
    public string ChatId { get; set; }
    public string AudioId { get; set; }
}