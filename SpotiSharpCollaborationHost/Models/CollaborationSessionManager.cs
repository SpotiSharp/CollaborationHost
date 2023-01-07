namespace SpotiSharpCollaborationHost.Models;

public static class CollaborationSessionManager
{
    private static List<CollaborationSession> _collaborationSessions = new List<CollaborationSession>();
    public static List<CollaborationSession> CollaborationSessions
    {
        get { return _collaborationSessions; }
        set { _collaborationSessions = value; }
    }
}