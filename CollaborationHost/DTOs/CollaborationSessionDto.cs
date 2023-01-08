using CollaborationHost.Models;

namespace CollaborationHost.DTOs;

public class CollaborationSessionDto
{
    public string Id { get; set; }

    public List<string> SongIds { get; set; } = new List<string>();

    public List<string> FilteredSongIds { get; set; } = new List<string>();

    public CollaborationSessionDto(CollaborationSession collaborationSession)
    {
        Id = collaborationSession.Id;
        SongIds = collaborationSession.SongIds;
        FilteredSongIds = collaborationSession.FilteredSongIds;
    }
}