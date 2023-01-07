using SpotifyAPI.Web;
using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Models;

public static class DataCache
{
    public static List<Song> Songs { get; set; }
    public static List<FullArtist> Artists { get; set; }
}