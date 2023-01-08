using Microsoft.AspNetCore.Mvc;
using SpotiSharpBackend.Enums;
using SpotiSharpCollaborationHost.Interfaces;
using SpotiSharpCollaborationHost.Models;
using SpotiSharpCollaborationHost.Models.Filters;
using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Controllers;

[ApiController]
[Route("[controller]")]
public class CollaborationSessionController : ControllerBase
{
    [HttpPost("create-session")]
    public ActionResult CreateSession(string sessionId)
    {
        if (CheckSessionExists(out _, sessionId)) return BadRequest("Session already exists.");
        CollaborationSessionManager.CollaborationSessions.Add(new CollaborationSession(sessionId));
        return Ok("Created session.");
    }
    
    [HttpGet("get")]
    public ActionResult Get(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(session);
    }
    
    [HttpGet("get-songs")]
    public ActionResult GetSongsFromSession(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(DataCache.Songs.Where(s => session.SongIds.Contains(s.FullTrack.Id)));
    }
    
    [HttpGet("get-filtered-songs")]
    public ActionResult GetFilteredSongsFromSession(string sessionId, List<(TrackFilter, List<object>)> filterInputs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(DataCache.Songs.Where(s => session.FilteredSongIds.Contains(s.FullTrack.Id)));
    }

    [HttpGet("get-filters")]
    public ActionResult GetSessionFilters(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(DeserializeFilters(session.Filters));
    }

    [HttpPost("add-songs")]
    public ActionResult AddSongs(string sessionId, [FromBody] List<Song> songs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        session.SongIds.AddRange(songs.Select(s => s.FullTrack.Id));
        CacheManager.LoadSongDataForSession(songs);
        return Ok("Added songs to session");
    }

    [HttpPost("set-filters")]
    public ActionResult AddFilters(string sessionId, [FromBody] List<(TrackFilter, List<object>)> filterInputs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        session.Filters = SerializeFilters(filterInputs);
        
        return Ok("Filters have been set.");
    }

    [HttpPost("remove-songs")]
    public ActionResult RemoveSongs(string sessionId, [FromBody] List<string> songIds)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        foreach (string songId in songIds)
        {
            session.SongIds.Remove(songId);
        }

        return Ok("Removed songs from session");
    }

    private bool CheckSessionExists(out CollaborationSession? collaborationSession, string sessionId)
    {
        collaborationSession = CollaborationSessionManager.CollaborationSessions.FirstOrDefault(cs => cs.Id == sessionId);
        return collaborationSession != null;
    }

    private List<IFilter> SerializeFilters(List<(TrackFilter, List<object>)> filterInputs)
    {
        var filters = new List<IFilter>();
        foreach (var filterInput in filterInputs)
        {
            IFilter filter = new TextFilter(TrackFilter.Genre, "");
            switch (filterInput.Item1)
            {
                case TrackFilter.Genre:
                    filter = new TextFilter(filterInput.Item1, (string)filterInput.Item2[0]);
                    break;
                case TrackFilter.Popularity:
                case TrackFilter.Danceability:
                case TrackFilter.Energy:
                case TrackFilter.Positivity:
                    filter = new RangeFilter(filterInput.Item1, (NumericFilterOption)filterInput.Item2[0], (double)filterInput.Item2[1]);
                    break;
                case TrackFilter.Tempo:
                    filter = new NumberFilter(filterInput.Item1, (NumericFilterOption)filterInput.Item2[0], (int)filterInput.Item2[1]); 
                    break;
            }
            filters.Add(filter);
        }

        return filters;
    }

    private List<(TrackFilter, List<object>)> DeserializeFilters(List<IFilter> filterInputs)
    {
        var filters = new List<(TrackFilter, List<object>)>();
        foreach (var filterInput in filterInputs)
        {
            (TrackFilter, List<object>) filter = (TrackFilter.Genre, new List<object>());
            switch (filterInput)
            {
                case TextFilter textFilter:
                    filter = (textFilter.TrackFilter, new List<object>{textFilter.GenreName});
                    break;
                case RangeFilter rangeFilter:
                    filter = (rangeFilter.TrackFilter, new List<object>{rangeFilter.NumericFilterOption, rangeFilter.RangeValue});
                    break;
                case NumberFilter numberFilter:
                    filter = (numberFilter.TrackFilter, new List<object>{numberFilter.NumericFilterOption, numberFilter.NumberValue});
                    break;
            }
            filters.Add(filter);
        }

        return filters;
    }
}