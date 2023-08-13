// ReSharper disable InconsistentNaming

// ReSharper disable ClassNeverInstantiated.Global
namespace DLL;

public abstract class ResponseBase
{
    public int pageNumber { get; init; }
    public int pageSize { get; init; }
    public bool pageFound { get; init; }
    public int total { get; init; }
    public int records { get; init; }
    public bool succeeded { get; init; }
}

public record Channel
{
    public string? idFromEprog { get; init; }
    public string? name { get; init; }
    public string? logo { get; init; }
    public string? baseLogo { get; init; }
    public long eventsCount { get; init; }
    public string? id { get; init; }
}

public record Event
{
    public string? eventId { get; init; }
    public string? title { get; init; }
    public string? description { get; init; }
    public string? eventStartTime { get; init; }

    public TimeOnly _eventStartTime => TimeOnly.Parse(eventStartTime!);

    public string? eventEndTime { get; init; }

    public TimeOnly _eventEndTime => TimeOnly.Parse(eventEndTime!);

    public string? channelName { get; init; }
    public string? eventInitialDateTime { get; init; }

    public DateTime _eventInitialDateTime => DateTime.Parse(eventInitialDateTime!);

    public string? eventEndDateTime { get; init; }

    public DateTime _eventEndDateTime => DateTime.Parse(eventEndDateTime!);

    public string? id { get; init; }
}

public class ResponseChannel : ResponseBase
{
    public Channel[]? results { get; set; }
}

public class ResponseEvent : ResponseBase
{
    public Event[]? results { get; set; }
}