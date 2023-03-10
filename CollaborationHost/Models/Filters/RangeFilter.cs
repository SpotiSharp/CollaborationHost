using SpotiSharpBackend;
using SpotiSharpBackend.Enums;
using CollaborationHost.Interfaces;

namespace CollaborationHost.Models.Filters;

public class RangeFilter : BaseFilter, IFilter
{      
    public NumericFilterOption NumericFilterOption { get; set; }
    public double RangeValue { get; set; }

    public RangeFilter(TrackFilter trackFilter, Guid guid, NumericFilterOption numericFilterOption = NumericFilterOption.Equal, double rangeValue = 0.0) : base(trackFilter, guid)
    {
        NumericFilterOption = numericFilterOption;
        RangeValue = rangeValue;
    }

    public async Task<List<SongData>> FilterSongs(List<SongData> songs)
    {
        var filteredSongs = (await TrackFilter.GetFilterFunction()(songs.Select(s => s.FullTrack).ToList(), songs.Select(s => s.AudioFeatures).ToList(), Convert.ToInt32(RangeValue), NumericFilterOption)).Select(s => s.Id);

        return songs.Where(s => filteredSongs.Contains(s.FullTrack.Id)).ToList();
    }
}