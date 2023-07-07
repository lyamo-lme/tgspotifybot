namespace SpotifyTelegramBot.Models;

public class SongDto
{
    public string UrlUtube { get; set; }
    public string? ImageUrl { get; set; }
    public string Name { get; set; }
    public int DurationS { get; set; }
    public string[] Artists { get; set; }
}