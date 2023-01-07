using SpotiSharpCollaborationHost.Models.Spotify;

namespace SpotiSharpCollaborationHost.Models;

public class CollaborationSession
{
    public string Id { get; set; }
    
    public List<Song> Songs { get; set; }

    public CollaborationSession(string sessionId)
    {
        Id = sessionId;
    }
}