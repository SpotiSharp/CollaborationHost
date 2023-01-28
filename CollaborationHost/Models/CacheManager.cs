using SpotifyAPI.Web;
using SpotiSharpBackend;

namespace CollaborationHost.Models;

public static class CacheManager
{
    static CacheManager()
    {
        DataRefreshLoop.Instance.OnDataRefresh += UpdateCachedData;
    }

    public static async Task<List<SongData>> GetSongsById(List<string> songIds)
    {
        var resultSongs = new List<SongData>();
        foreach (string songId in songIds)
        {

            if (DataCache.Songs.Select(s => s.FullTrack?.Id).Contains(songId))
            {
                resultSongs.Add(DataCache.Songs.First(s => s.FullTrack.Id == songId));
                continue;
            }
            var apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            var fullTrack = apiCallerInstance?.GetTrackById(songId);
            apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            var audioFeatures = apiCallerInstance?.GetAudioFeaturesByTrackId(songId);
            if (fullTrack == null || audioFeatures == null) continue;
            resultSongs.Add(new SongData
            {
                FullTrack = fullTrack,
                AudioFeatures = audioFeatures
            });
        }

        return resultSongs;
    }

    public static async void LoadSongDataForSession(List<string> songIds)
    {
        var cachedSongIds = DataCache.Songs.Select(s => s.FullTrack.Id);
        var songIdsNew = songIds.Where(song => !cachedSongIds.Contains(song)).ToList();

        var fullTracks = APICaller.Instance?.GetMultipleTracksByTrackId(songIdsNew);
        if (fullTracks == null) return;
        var audioFeatures = APICaller.Instance?.GetMultipleTrackAudioFeaturesByTrackIds(fullTracks.Select(ft => ft.Id).ToList());
        var artists = new List<FullArtist>();
        List<SongData> loadedSongs = fullTracks.Select(s => new SongData 
            {
                FullTrack = s,
                AudioFeatures = audioFeatures?.First(af => af.Id == s.Id)
            }
        ).ToList();

        var cachedArtistsIds = DataCache.Artists.Select(fa => fa.Id);
        foreach (FullTrack song in fullTracks)
        {
            if (song == null) continue;
            foreach (var artist in song.Artists)
            {
                if (cachedArtistsIds.Contains(artist.Id)) continue;
                var apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
                var loadedArtist = apiCallerInstance?.GetArtistById(artist.Id);
                if(loadedArtist != null) artists.Add(loadedArtist);
                
            }
        }
        
        DataCache.Artists.AddRange(artists);
        DataCache.Songs.AddRange(loadedSongs);
    }

    private static async void UpdateCachedData()
    {
        var tmpSongs = DataCache.Songs;
        var tmpArtists = DataCache.Artists;
        DataCache.IsLocked = true;
        
        for (int i = 0; i < tmpSongs.Count; i++)
        {
            var apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            tmpSongs[i].FullTrack = apiCallerInstance?.GetTrackById(tmpSongs[i].FullTrack?.Id) ?? tmpSongs[i].FullTrack;
            apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            tmpSongs[i].AudioFeatures = apiCallerInstance?.GetAudioFeaturesByTrackId(tmpSongs[i].FullTrack?.Id) ?? tmpSongs[i].AudioFeatures;
        }
        for (int i = 0; i < tmpArtists.Count; i++)
        {
            var apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            tmpArtists[i] = apiCallerInstance?.GetArtistById(tmpArtists[i].Id) ?? tmpArtists[i];
        }
        
        DataCache._songs = tmpSongs;
        DataCache._artists = tmpArtists;
        
        DataCache.IsLocked = false;
    }
}