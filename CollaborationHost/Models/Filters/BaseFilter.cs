using SpotiSharpBackend.Enums;

namespace CollaborationHost.Models.Filters;

public class BaseFilter
{
    public TrackFilter TrackFilter { get; private set; }

    public BaseFilter(TrackFilter trackFilter)
    {
        TrackFilter = trackFilter;
    }
}