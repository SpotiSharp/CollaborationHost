using SpotiSharpBackend;

namespace SpotiSharpCollaborationHost.Interfaces;

public interface IFilter
{
    public Task<List<SongData>> FilterSongs(List<SongData> songs);
}