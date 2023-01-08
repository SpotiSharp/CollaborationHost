using CollaborationHost.Interfaces;

namespace CollaborationHost.Models;

public class CollaborationSession
{
    public string Id { get; set; }

    public List<string> SongIds { get; set; } = new List<string>();

    public List<string> FilteredSongIds { get; set; } = new List<string>();

    public List<IFilter> Filters { get; set; } = new List<IFilter>();

    public CollaborationSession(string sessionId)
    {
        Id = sessionId;
    }

    public async void UpdateFilteredSongs()
    {
        var tmpSongs = await CacheManager.GetSongsById(SongIds);
        foreach (var filter in Filters)
        {
            tmpSongs = await filter.FilterSongs(tmpSongs);
        }
        FilteredSongIds = tmpSongs.Select(ts => ts.FullTrack.Id).ToList();
    }
}