using SpotiSharpBackend;
using SpotiSharpBackend.Enums;
using SpotiSharpCollaborationHost.Interfaces;

namespace SpotiSharpCollaborationHost.Models.Filters;

public class TextFilter : BaseFilter, IFilter
{
    public string GenreName { get; set; }
    
    public TextFilter(TrackFilter trackFilter, string genreName) : base(trackFilter)
    {
        GenreName = genreName;
    }

    public async Task<List<SongData>> FilterSongs(List<SongData> songs)
    {
        var filteredSongs = (await TrackFilter.GetFilterFunction()(songs.Select(s => s.FullTrack).ToList(), songs.Select(s => s.AudioFeatures).ToList(), GenreName)).Select(s => s.Id).ToList();

        return songs.Where(s => filteredSongs.Contains(s.FullTrack.Id)).ToList();
    }
}