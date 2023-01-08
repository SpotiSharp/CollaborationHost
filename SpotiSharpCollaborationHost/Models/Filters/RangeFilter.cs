using SpotiSharpBackend;
using SpotiSharpBackend.Enums;
using SpotiSharpCollaborationHost.Interfaces;

namespace SpotiSharpCollaborationHost.Models.Filters;

public class RangeFilter : BaseFilter, IFilter
{      
    public NumericFilterOption NumericFilterOption { get; set; }
    public double RangeValue { get; set; }

    public RangeFilter(TrackFilter trackFilter, NumericFilterOption numericFilterOption = NumericFilterOption.Equal, double rangeValue = 0.0) : base(trackFilter)
    {
        NumericFilterOption = numericFilterOption;
        RangeValue = rangeValue;
    }

    public async Task<List<SongData>> FilterSongs(List<SongData> songs)
    {
        var filteredSongs = (await TrackFilter.GetFilterFunction()(songs.Select(s => s.FullTrack).ToList(), songs.Select(s => s.AudioFeatures).ToList(), RangeValue, NumericFilterOption)).Select(s => s.Id).ToList();

        return songs.Where(s => filteredSongs.Contains(s.FullTrack.Id)).ToList();
    }
}