using Microsoft.AspNetCore.Mvc;
using SpotiSharpCollaborationHost.Models;
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

    [HttpPost("add-songs")]
    public ActionResult AddSongs(string sessionId, [FromBody] List<Song> songs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        session.Songs.AddRange(songs);
        return Ok("Added songs to session");
    }

    [HttpPost("remove-songs")]
    public ActionResult RemoveSongs(string sessionId, [FromBody] List<Song> songs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return BadRequest("Session doesn't exist.");
        foreach (var song in songs)
        {
            session.Songs.Remove(song);
        }
        return Ok("Removed songs from session");
    }
    
    private bool CheckSessionExists(out CollaborationSession? collaborationSession, string sessionId)
    {
        collaborationSession = null;
        var collaborationSessions = CollaborationSessionManager.CollaborationSessions.Where(cs => cs.Id == sessionId).ToList();
        
        if (collaborationSessions.Count <= 0) return false;
        
        collaborationSession = collaborationSessions[0];
        return true;
    }
}