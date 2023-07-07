using System.Text;
using SpotifyAPI.Web;
using SpotifyTelegramBot.Extensions;
using SpotifyTelegramBot.Models;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Services;


namespace SpotifyTelegramBot.SpotifyDownloader;

public class SpotifyProvider
{
    private readonly SpotifyClient _spotifyClient;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5, 5);

    public SpotifyProvider()
    {
        var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(
                new ClientCredentialsAuthenticator(
                    "15c242fd9ad74b6cb57168d61e67cabd",
                    "c32b3fbd46df4c59a98b9499c12c6986"
                ));

        _spotifyClient = new SpotifyClient(config);
    }


    public async Task<List<SongDto>> GetLinks(string findValue)
    {
        var playlist = await _spotifyClient.Playlists.Get(findValue);
        var tracks = await _spotifyClient.PaginateAll(playlist.Tracks);
        List<SongDto> urls = new List<SongDto>();
        var tasks = tracks.Select(async track =>
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (track.Track is FullTrack trackItem)
                {
                    using var httpClient = new HttpClient();
                    YoutubeSearchClient client = new YoutubeSearchClient(httpClient);
                    var responseObject = await client.SearchAsync(trackItem.QueryStringUtube());
                    urls.Add(new SongDto()
                    {
                        UrlUtube = responseObject.Results.First().Url,
                        Name = trackItem.Name,
                        ImageUrl = "",
                        DurationS = trackItem.DurationMs*1000,
                        Artists = trackItem.Artists.Select(x => x.Name).ToArray()
                    });
                    Console.WriteLine(responseObject.Results.First().Url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        });
        await Task.WhenAll(tasks.ToList());

        return urls;
    }
    //
    // public async Task<List<DataFile>> PinImageToAudio(DataFile)
    // {
    // }
}