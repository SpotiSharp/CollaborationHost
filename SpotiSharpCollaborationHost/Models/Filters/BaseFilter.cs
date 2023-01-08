using SpotiSharpBackend.Enums;

namespace SpotiSharpCollaborationHost.Models.Filters;

public class BaseFilter
{
    public TrackFilter TrackFilter { get; private set; }

    public BaseFilter(TrackFilter trackFilter)
    {
        TrackFilter = trackFilter;
    }
}