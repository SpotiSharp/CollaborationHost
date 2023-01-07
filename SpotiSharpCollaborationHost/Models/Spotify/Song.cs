using SpotifyAPI.Web;

namespace SpotiSharpCollaborationHost.Models.Spotify;

public class Song
{
    public FullTrack FullTrack { get; set; }
    public List<string> ArtistIds { get; set; }
}