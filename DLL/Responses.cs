// ReSharper disable InconsistentNaming

namespace DLL;

public abstract class ResponseBase
{
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public bool pageFound { get; set; }
    public int total { get; set; }
    public int records { get; set; }
    public bool succeeded { get; set; }
}

public record Channel
{
    public string? idFromEprog { get; set; }
    public string? name { get; set; }
    public string? logo { get; set; }
    public string? baseLogo { get; set; }
    public long eventsCount { get; set; }
    public string? id { get; set; }
}

//TODO: Makes the get properties to convert string to its params
public record Event
{
    public string? eventId { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public string? eventStartTime { get; set; }

    public TimeOnly _eventStartTime
    {
        get
        {
            var args = eventStartTime!.Split(':');
            return new TimeOnly(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
        }
    }

    public string? eventEndTime { get; set; }

    public TimeOnly _eventEndTime
    {
        get
        {
            var args = eventEndTime!.Split(':');
            return new TimeOnly(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
        }
    }

    public string? channelName { get; set; }
    public string? eventInitialDateTime { get; set; }

    public DateTime _eventInitialDateTime
    {
        get
        {
            var args = eventInitialDateTime!.Split(':');
            return new DateTime(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
        }
    }

    public string? eventEndDateTime { get; set; }

    public DateTime _eventEndDateTime
    {
        get
        {
            var args = eventEndDateTime!.Split(':');
            return new DateTime(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
        }
    }

    public string? id { get; set; }
}

public class ResponseChannel : ResponseBase
{
    public Channel[]? results { get; set; }
}

public class ResponseEvent : ResponseBase
{
    public Event[]? results { get; set; }
}