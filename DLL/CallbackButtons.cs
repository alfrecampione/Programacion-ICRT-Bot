using Telegram.Bot.Types.ReplyMarkups;

namespace DLL;

public static class CallbackButtons
{
    public static InlineKeyboardMarkup Channels = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Cubavision", "Cubavision")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Tele Rebelde", "Tele Rebelde")
            }
        }
    );

    public static InlineKeyboardMarkup Start = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Channels List", "Channels List")
            }
        }
    );
}