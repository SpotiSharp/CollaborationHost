using SpotiSharpBackend.Enums;

namespace CollaborationHost.Models.Filters;

public class BaseFilter
{
    public Guid Guid { get; set; }
    public TrackFilter TrackFilter { get; private set; }

    public BaseFilter(TrackFilter trackFilter, Guid guid)
    {
        TrackFilter = trackFilter;
        Guid = guid;
    }
}