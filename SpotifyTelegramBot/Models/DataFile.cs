namespace SpotifyTelegramBot.Models;

public class DataFile
{
    public Stream stream;

    public byte[] Data
    {
        get
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }

    public string title;
    public int duration;
    public string artist;
    public string typeFile;
}