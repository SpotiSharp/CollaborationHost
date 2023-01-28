﻿using SpotiSharpBackend;
using SpotiSharpBackend.Enums;
using CollaborationHost.Interfaces;

namespace CollaborationHost.Models.Filters;

public class NumberFilter : BaseFilter, IFilter
{ 
    public NumericFilterOption NumericFilterOption { get; set; }
    public int NumberValue { get; set; }

    public NumberFilter(TrackFilter trackFilter, NumericFilterOption numericFilterOption = NumericFilterOption.Equal, int numberValue = 0) : base(trackFilter)
    {
        NumericFilterOption = numericFilterOption;
        NumberValue = numberValue;
    }
    
    public async Task<List<SongData>> FilterSongs(List<SongData> songs)
    {
        var filteredSongs = (await TrackFilter.GetFilterFunction()(songs.Select(s => s.FullTrack).ToList(), songs.Select(s => s.AudioFeatures).ToList(), NumberValue, NumericFilterOption)).Select(s => s.Id).ToList();

        return songs.Where(s => filteredSongs.Contains(s.FullTrack.Id)).ToList();
    }
}