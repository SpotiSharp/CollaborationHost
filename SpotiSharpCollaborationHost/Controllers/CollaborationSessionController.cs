using Microsoft.AspNetCore.Mvc;
using SpotiSharpCollaborationHost.Models;
using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Controllers;

[ApiController]
[Route("[controller]")]
public class CollaborationSessionController : ControllerBase
{
    public bool CreateSession(string sessionId)
    {
        if (CheckSessionExists(out _, sessionId)) return false;
        CollaborationSessionManager.CollaborationSessions.Add(new CollaborationSession(sessionId));
        return true;
    }

    public bool AddSongs(string sessionId, List<Song> songs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return false;
        session.Songs.AddRange(songs);
        return true;
    }

    public bool RemoveSongs(string sessionId, List<Song> songs)
    {
        if (!CheckSessionExists(out CollaborationSession? session, sessionId)) return false;
        foreach (var song in songs)
        {
            session.Songs.Remove(song);
        }
        return true;
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