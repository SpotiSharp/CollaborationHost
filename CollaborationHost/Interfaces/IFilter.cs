using SpotiSharpBackend;

namespace CollaborationHost.Interfaces;

public interface IFilter
{
    public Task<List<SongData>> FilterSongs(List<SongData> songs);
}