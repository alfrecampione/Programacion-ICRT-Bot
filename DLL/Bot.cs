using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DLL;

public class Bot
{
    private const string BasePath = "https://eprog.importer.premiumtesh.nat.cu/api-data";
    private const string ChannelPath = "/channel";
    private const string EventPath = "/event";
    private static Dictionary<long, List<int>>? _channelButtonsPerUser;

    public Bot(string token)
    {
        var myBot = new TelegramBotClient(token);
        _channelButtonsPerUser = new Dictionary<long, List<int>>();
        this.StartReceiver(myBot);
    }

    private async Task StartReceiver(ITelegramBotClient? botClient)
    {
        var cancellationTokenSource = new CancellationTokenSource().Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        if (botClient != null)
            await botClient.ReceiveAsync(OnMessage, ErrorMessage, receiverOptions, cancellationTokenSource);
    }

    private static async Task OnMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Text: "/start" } message)
        {
            Console.WriteLine("Chat iniciado con: " + message.From!.Username);
            await bot.SendTextMessageAsync(message.From!.Id, "Welcome to the bot",
                replyMarkup: CallbackButtons.Start,
                cancellationToken: cancellationToken);
        }

        if (update.CallbackQuery is { } callbackQuery)
        {
            var args = callbackQuery.Data?.Split("-");

            switch (args?[0])
            {
                case "Channel List":
                    Console.WriteLine(callbackQuery.Message!.Chat.Username + " Channel List");
                    var page = (args.Length > 1) ? int.Parse(args[1]) : 1;
                    await DeleteChannels(bot, callbackQuery.From!.Id, cancellationToken);
                    await DeleteSource(bot, callbackQuery, cancellationToken);
                    await SendChannelListWithPhotos(bot, callbackQuery.From.Id, page, cancellationToken);
                    break;

                case "Event List":
                    if (args.Length >= 3)
                    {
                        if (!DateTime.TryParse(args[2], out var dateTime))
                            dateTime = DateTime.Today;
                        Console.WriteLine(callbackQuery.Message!.Chat.Username + $" Event List for {args[1]}");
                        await DeleteChannels(bot, callbackQuery.From!.Id, cancellationToken);
                        await DeleteSource(bot, callbackQuery, cancellationToken);
                        await SendEventList(bot, callbackQuery.From.Id, args[1], dateTime, cancellationToken);
                    }

                    break;

                case "Event":
                    if (args.Length >= 2)
                    {
                        var id = string.Join("-", args.Skip(1));
                        Console.WriteLine(callbackQuery.Message!.Chat.Username + $" Event selected");
                        await bot.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                            cancellationToken: cancellationToken);
                        await CallbackButtons.SendEvent(bot, BasePath, EventPath, id, callbackQuery.Message!.Chat.Id);
                    }

                    break;

                case "Start":
                    Console.WriteLine("Chat iniciado con: " + callbackQuery.Message!.Chat.Username);
                    await DeleteChannels(bot, callbackQuery.From!.Id, cancellationToken);
                    await DeleteSource(bot, callbackQuery, cancellationToken);
                    await bot.SendTextMessageAsync(callbackQuery.From!.Id, "Welcome to the bot",
                        replyMarkup: CallbackButtons.Start,
                        cancellationToken: cancellationToken);
                    break;
            }
        }
    }

    private static async Task SendChannelList(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        await bot.SendTextMessageAsync(chatId, "Choose a channel",
            replyMarkup: CallbackButtons.GetChannels(BasePath, ChannelPath, 0, "sorts=name"),
            cancellationToken: cancellationToken);
    }

    private static async Task SendChannelListWithPhotos(ITelegramBotClient bot, long chatId, int page,
        CancellationToken cancellationToken)
    {
        var (buttons, photos) = CallbackButtons.GetChannelsWithPhoto(BasePath, ChannelPath, page,
            "sorts=name", $"page={page}", $"pageSize=10");
        var photoList = photos.ToList();

        await DeleteChannels(bot, chatId, cancellationToken);

        var channelsID = new List<int>();
        foreach (var photo in photos)
        {
            var channelMessage = await bot.SendPhotoAsync(chatId, photo,
                replyMarkup: new InlineKeyboardMarkup(buttons[photoList.IndexOf(photo)]),
                cancellationToken: cancellationToken);
            channelsID.Add(channelMessage.MessageId);
        }

        var markup = new InlineKeyboardMarkup(buttons.Skip(photos.Length));
        var optionsMessage = await bot.SendTextMessageAsync(chatId, "Other Options",
            replyMarkup: markup,
            cancellationToken: cancellationToken);
        channelsID.Add(optionsMessage.MessageId);

        _channelButtonsPerUser?.Add(chatId, channelsID);
    }

    private static async Task SendEventList(ITelegramBotClient bot, long chatId, string channelName, DateTime dateTime,
        CancellationToken cancellationToken)
    {
        var reply = CallbackButtons.GetEventList(BasePath, EventPath, channelName, dateTime,
            "allrecords=1",
            $"filters=channelName=={channelName},eventInitialDateTime>={dateTime:yyyy-MM-dd}T04:00:00Z,eventInitialDateTime<={dateTime.AddDays(1):yyyy-MM-dd}T03:59:59Z",
            "sorts=eventInitialDateTime");

        await bot.SendTextMessageAsync(chatId,
            $"{channelName}--{dateTime.Day}/{dateTime.Month}/{dateTime.Year}",
            replyMarkup: reply,
            cancellationToken: cancellationToken);
    }

    private static async Task DeleteChannels(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        try
        {
            if (!_channelButtonsPerUser!.TryGetValue(chatId, out var channelsID))
                return;

            foreach (var messageId in channelsID)
            {
                await bot.DeleteMessageAsync(chatId, messageId, cancellationToken: cancellationToken);
            }

            _channelButtonsPerUser.Remove(chatId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task DeleteSource(ITelegramBotClient bot, CallbackQuery sourceMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            await bot.DeleteMessageAsync(sourceMessage.From!.Id, sourceMessage.Message!.MessageId,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
        }
    }

    private static async Task ErrorMessage(ITelegramBotClient bot, Exception e, CancellationToken cancellationToken)
    {
        if (e is ApiRequestException requestException)
            await bot.SendTextMessageAsync(0, $"Error: {requestException.ErrorCode} - {requestException.Message}",
                cancellationToken: cancellationToken);
    }
}