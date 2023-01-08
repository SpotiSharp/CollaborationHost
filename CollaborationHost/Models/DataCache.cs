using SpotifyAPI.Web;
using SpotiSharpBackend;

namespace CollaborationHost.Models;

public static class DataCache
{
    private static int _millisecondRetryDelay = 50;
    
    public static List<SongData> _songs = new List<SongData>();
    public static List<SongData> Songs
    {
        get
        {
            while (IsLocked)
            {
                Thread.Sleep(_millisecondRetryDelay);
            }
            return _songs;
        }
        set
        {
            while (IsLocked)
            {
                Thread.Sleep(_millisecondRetryDelay);
            }
            _songs = value;
        }
    }

    public static List<FullArtist> _artists = new List<FullArtist>();
    public static List<FullArtist> Artists
    {
        get
        {
            while (IsLocked)
            {
                Thread.Sleep(_millisecondRetryDelay);
            }
            return _artists;
        }
        set
        {
            while (IsLocked)
            {
                Thread.Sleep(_millisecondRetryDelay);
            }
            _artists = value;
        }
    }

    public static bool IsLocked { get; set; } = false;
}