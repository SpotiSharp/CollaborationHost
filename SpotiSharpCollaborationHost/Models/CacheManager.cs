using SpotifyAPI.Web;
using SpotiSharpBackend;
using SpotiSharpCollaborationHost.Controllers;
using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Models;

public static class CacheManager
{
    static CacheManager()
    {
        DataRefreshLoop.Instance.OnDataRefresh += UpdateCachedData;
    }

    public static async Task<List<Song>> GetSongsById(List<string> songIds)
    {
        var resultSongs = new List<Song>();
        foreach (string songId in songIds)
        {
            
            if (DataCache.Songs.Select(s => s.FullTrack?.Id).Contains(songId)) resultSongs.Add(DataCache.Songs.First(s => s.FullTrack.Id == songId));
            var apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            var fullTrack = apiCallerInstance?.GetTrackById(songId);
            apiCallerInstance = await APICaller.WaitForRateLimitWindowInstance;
            var audioFeatures = apiCallerInstance?.GetAudioFeaturesByTrackId(songId);
            if (fullTrack == null || audioFeatures == null) continue;
            resultSongs.Add(new Song
            {
                FullTrack = fullTrack,
                AudioFeatures = audioFeatures
            });
        }

        return resultSongs;
    }

    public static async void LoadSongDataForSession(List<Song> songs)
    {
        var audioFeatures = APICaller.Instance?.GetMultipleTrackAudioFeaturesByTrackIds(songs.Select(s => s.FullTrack.Id).ToList());
        var artists = new List<FullArtist>();
        List<Song> loadedSongs = songs.Select(s => new Song 
            {
                FullTrack = s.FullTrack,
                AudioFeatures = audioFeatures?.First(af => af.Id == s.FullTrack?.Id)
            }
        ).ToList();

        foreach (Song song in songs)
        {
            if (song.FullTrack == null) continue;
            foreach (var artist in song.FullTrack.Artists)
            {
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