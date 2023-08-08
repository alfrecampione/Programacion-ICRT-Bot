using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DLL;

public static class CallbackButtons
{
    public static InlineKeyboardMarkup GetChannels(string basePath, string endpoint, int page, params string[] args)
    {
        // ReSharper disable once StringLiteralTypo
        var fullPath = basePath + endpoint + "?allrecords=1";
        fullPath = args.Aggregate(fullPath, (current, param) => current + ("&" + param));

        var channels = HttpConnection<ResponseChannel>.GetResponseAsync(fullPath).Result?.results;
        var buttons = new List<List<InlineKeyboardButton>>();

        if (channels == null) return new InlineKeyboardMarkup(buttons);

        var channelsToShow = channels.Chunk(10).Skip(page).First();
        buttons.AddRange(from channel in channelsToShow
            where channel.name != null && channel.logo != null
            select new List<InlineKeyboardButton>()
                { InlineKeyboardButton.WithCallbackData(channel.name, $"Channel-{channel.name}") });

        buttons.Add(CreateChangePageButtons(page, page > 0, page < channels.Length / 10,"Channel List"));

        buttons.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Back", "Start") });
        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetEventRange(string basePath, string endpoint,string channelName,
        int page, params string[] args)
    {
        var fullPath = basePath + endpoint +"?allrecords=1";
        fullPath = args.Aggregate(fullPath, (current, param) => current + ("&" + param));

        var events = HttpConnection<ResponseEvent>.GetResponseAsync(fullPath).Result?.results;
        var buttons = new List<List<InlineKeyboardButton>>();

        if (events == null) return new InlineKeyboardMarkup(buttons);

        var distincDates = events.Where((t, i) => i > 0 && events[i - 1]._eventInitialDateTime.Date != t._eventInitialDateTime.Date)
            .Select(e => e._eventInitialDateTime).ToArray();
        var datesToShow = distincDates.Chunk(10).Skip(page).First();
        
        buttons.AddRange(from date in datesToShow
            select new List<InlineKeyboardButton>()
                { InlineKeyboardButton.WithCallbackData($"{date.Day}/{date.Month}", $"Date-{channelName}-{date}") });
        
        buttons.Add(CreateChangePageButtons(page,page>0,page<distincDates.Count()/10,$"Date List-{channelName}"));
        
        buttons.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Back", "Channel List") });
        return new InlineKeyboardMarkup(buttons);
    }

    private static List<InlineKeyboardButton> CreateChangePageButtons(int page, bool back, bool next,string callbackData)
    {
        List<InlineKeyboardButton> buttonsList = new();
        if (back)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏪", $"{callbackData}-{page - 1}"));
        if (next)
            buttonsList.Add(InlineKeyboardButton.WithCallbackData("⏩", $"{callbackData}-{page + 1}"));
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