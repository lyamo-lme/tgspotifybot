using System.Text;
using SpotifyAPI.Web;
using SpotifyTelegramBot.Models;
using YoutubeSearchApi.Net.Services;

namespace SpotifyTelegramBot.Extensions;

public static class FullTrackExtensions
{
    public static string QueryStringUtube(this FullTrack trackItem)
    {
        StringBuilder queryBuilder = new StringBuilder();
        foreach (var artist in trackItem.Artists)
        {
            queryBuilder.Append(artist.Name);
            queryBuilder.Append(", ");
        }

        queryBuilder.Append(trackItem.Name);
        queryBuilder.Append(" audio");
        return queryBuilder.ToString();
    }

    public static string QueryStringUtube(this SimpleTrack trackItem)
    {
        StringBuilder queryBuilder = new StringBuilder();
        foreach (var artist in trackItem.Artists)
        {
            queryBuilder.Append(artist.Name);
            queryBuilder.Append(", ");
        }

        queryBuilder.Append(trackItem.Name);
        queryBuilder.Append(" audio");
        return queryBuilder.ToString();
    }
}