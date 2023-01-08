using Microsoft.AspNetCore.Mvc;
using SpotiSharpBackend.Enums;
using CollaborationHost.DTOs;
using CollaborationHost.Interfaces;
using CollaborationHost.Models;
using CollaborationHost.Models.Filters;

namespace CollaborationHost.Controllers;

[ApiController]
[Route("[controller]")]
public class CollaborationSessionController : ControllerBase
{
    [HttpGet("get")]
    public ActionResult Get(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(new CollaborationSessionDto(session));
    }
    
    [HttpGet("get-songs")]
    public ActionResult GetSongsFromSession(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        var resultsongs = DataCache.Songs.Where(s => session.SongIds.Contains(s.FullTrack.Id));
        return Ok(resultsongs);
    }
    
    [HttpGet("get-filtered-songs")]
    public ActionResult GetFilteredSongsFromSession(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        
        var resultsongs = DataCache.Songs.Where(s => session.FilteredSongIds.Contains(s.FullTrack.Id));
        return Ok(resultsongs);
    }

    [HttpPost("filter-songs")]
    public ActionResult FilterSongsFromSession(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        session.UpdateFilteredSongs();
        return Ok("Songs have been filtered.");
    }

    [HttpGet("get-filters")]
    public ActionResult GetSessionFilters(string sessionId)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        return Ok(DeserializeFilters(session.Filters));
    }
    
    [HttpPost("create-session")]
    public ActionResult CreateSession(string sessionId)
    {
        if (CheckSessionExists(out _, sessionId)) return BadRequest("Session already exists.");
        CollaborationSessionManager.CollaborationSessions.Add(new CollaborationSession(sessionId));
        return Ok("Created session.");
    }

    [HttpPost("set-songs")]
    public ActionResult SetSongs(string sessionId, [FromBody] List<string> songIds)
    {
        if (!songIds.Any()) return BadRequest("Requires at least 1 song id.");
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        session.SongIds = songIds;
        CacheManager.LoadSongDataForSession(songIds);
        return Ok("Set songs for session");
    }

    [HttpPost("set-filters")]
    public ActionResult AddFilters(string sessionId, [FromBody] Dictionary<TrackFilter, List<object>> filterInputs)
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

    private List<IFilter> SerializeFilters(Dictionary<TrackFilter, List<object>> filterInputs)
    {
        var filters = new List<IFilter>();
        foreach (var filterInput in filterInputs)
        {
            IFilter filter = new TextFilter(TrackFilter.Genre, "");
            switch (filterInput.Key)
            {
                case TrackFilter.Genre:
                    filter = new TextFilter(filterInput.Key, (string)filterInput.Value[0]);
                    break;
                case TrackFilter.Popularity:
                case TrackFilter.Danceability:
                case TrackFilter.Energy:
                case TrackFilter.Positivity:
                    filter = new RangeFilter(filterInput.Key, (NumericFilterOption)filterInput.Value[0], (double)filterInput.Value[1]);
                    break;
                case TrackFilter.Tempo:
                    filter = new NumberFilter(filterInput.Key, (NumericFilterOption)filterInput.Value[0], (int)filterInput.Value[1]); 
                    break;
            }
            filters.Add(filter);
        }

        return filters;
    }

    private Dictionary<TrackFilter, List<object>> DeserializeFilters(List<IFilter> filterInputs)
    {
        var filters = new Dictionary<TrackFilter, List<object>>();
        foreach (var filterInput in filterInputs)
        {
            KeyValuePair<TrackFilter, List<object>> filter = new KeyValuePair<TrackFilter, List<object>>();
            switch (filterInput)
            {
                case TextFilter textFilter:
                    filter = new KeyValuePair<TrackFilter, List<object>>(textFilter.TrackFilter, new List<object>{textFilter.GenreName});
                    break;
                case RangeFilter rangeFilter:
                    filter = new KeyValuePair<TrackFilter, List<object>>(rangeFilter.TrackFilter, new List<object>{rangeFilter.NumericFilterOption, rangeFilter.RangeValue});
                    break;
                case NumberFilter numberFilter:
                    filter = new KeyValuePair<TrackFilter, List<object>>(numberFilter.TrackFilter, new List<object>{numberFilter.NumericFilterOption, numberFilter.NumberValue});
                    break;
            }
            
            filters.Add(filter.Key, filter.Value);
        }

        return filters;
    }
}