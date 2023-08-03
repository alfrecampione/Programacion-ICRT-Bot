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
    public ulong rowVersion { get; set; }
    public string? createdBy { get; set; }
    public string? createdById { get; set; }
    public DateTime createdDate { get; set; }
    public string? modifiedBy { get; set; }
    public string? modifiedById { get; set; }
    public DateTime modifiedDate { get; set; }
    public bool generateMetaDataFields { get; set; }
}

public record Event
{
    public string? eventId { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public DateOnly eventInitialDate { get; set; }
    public DateOnly eventEndDate { get; set; }
    public string? idFromEprog { get; set; }
    public string? extendedDescription { get; set; }
    public string? transmission { get; set; }
    public string? pid { get; set; }
    public string? space { get; set; }
    public TimeOnly eventStartTime { get; set; }
    public TimeOnly eventEndTime { get; set; }
    public TimeOnly eventDuration { get; set; }
    public long eventDurationInSeconds { get; set; }
    public string? channelName { get; set; }
    public DateTime eventInitialDateTime { get; set; }
    public DateTime eventEndDateTime { get; set; }
    public bool isEventWithNegativeDuration { get; set; }
    public bool isEventWithDurationOver24Hrs { get; set; }
    public bool isEventWithTextOverLength { get; set; }
    public string? id { get; set; }
    public long rowVersion { get; set; }
    public string? createdBy { get; set; }
    public string? createdById { get; set; }
    public DateTime createdDate { get; set; }
    public string? modifiedBy { get; set; }
    public string? modifiedById { get; set; }
    public DateTime modifiedDate { get; set; }
    public bool generateMetaDataFields { get; set; }
}

public class ResponseChannel : ResponseBase
{
    public Channel[]? results { get; set; }
}
public class ResponseEvent : ResponseBase
{
    public Event[]? results { get; set; }
}