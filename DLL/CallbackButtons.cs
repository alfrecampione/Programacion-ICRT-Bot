using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DLL;

public static class CallbackButtons
{
    public static InlineKeyboardMarkup GetChannels(string basePath, string endpoint, int page, params string[] args)
    {
        var fullPath = basePath + endpoint;
        if (args.Length > 0)
        {
            fullPath += "?" + args[0];
            fullPath = args.Skip(1).Aggregate(fullPath, (current, param) => current + ("&" + param));
        }

        var responseChannels = HttpConnection<ResponseChannel>.GetResponseAsync(fullPath).Result;
        var channels = responseChannels?.results;
        var buttons = new List<List<InlineKeyboardButton>>();

        if (channels == null) return new InlineKeyboardMarkup(buttons);

        DateTime.Now.Deconstruct(out var actualDate, out _);

        buttons.AddRange(from channel in channels
            where channel.name != null && channel.logo != null
            select new List<InlineKeyboardButton>()
                { InlineKeyboardButton.WithCallbackData(channel.name, $"Event List-{channel.name}-{actualDate}") });

        buttons.Add(CreateChangePageButtons(page, page > 1, page < responseChannels?.total, "Channel List"));

        buttons.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Back", "Start") });
        return new InlineKeyboardMarkup(buttons);
    }

    //🔴 - Before
    //🟢 - Now 
    //🟡 - after
    public static InlineKeyboardMarkup GetEvent(string basePath, string endpoint, string channelName, DateTime date,
        params string[] args)
    {
        var fullPath = basePath + endpoint;
        if (args.Length > 0)
        {
            fullPath += "?" + args[0];
            fullPath = args.Skip(1).Aggregate(fullPath, (current, param) => current + ("&" + param));
        }

        var responseEvents = HttpConnection<ResponseEvent>.GetResponseAsync(fullPath).Result;
        var events = responseEvents?.results;
        var buttons = new List<List<InlineKeyboardButton>>();

        if (events != null)
        {
            string GetColor(Event actualEvent)
            {
                if (actualEvent._eventInitialDateTime > DateTime.Now)
                    return "🟡";
                if (actualEvent._eventEndDateTime < DateTime.Now)
                    return "🔴";
                return "🟢";
            }

            buttons.AddRange(from @event in events
                select new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"{GetColor(@event)}{@event._eventStartTime}->{@event.title}",
                        $"Event List-{channelName}-{date}")
                });
        }

        buttons.Add(CreateChangePageButtons(date, true, true, $"Event List-{channelName}"));

        buttons.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Back", "Channel List") });
        return new InlineKeyboardMarkup(buttons);
    }

    private static List<InlineKeyboardButton> CreateChangePageButtons(int page, bool back, bool next,
        string callbackData)
    {
        List<InlineKeyboardButton> buttonsList = new();
        if (back)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏪", $"{callbackData}-{page - 1}"));
        if (next)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏩", $"{callbackData}-{page + 1}"));
        return buttonsList;
    }

    private static List<InlineKeyboardButton> CreateChangePageButtons(DateTime day, bool back, bool next,
        string callbackData)
    {
        List<InlineKeyboardButton> buttonsList = new();
        if (back)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏪", $"{callbackData}-{day.AddDays(-1)}"));
        if (next)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏩", $"{callbackData}-{day.AddDays(1)}"));
        return buttonsList;
    }

    public static InlineKeyboardMarkup Start = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Channel List", "Channel List")
            }
        }
    );
}