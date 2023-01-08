using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Interfaces;

public interface IFilter
{
    public Task<List<Song>> FilterSongs(List<Song> songs);
}