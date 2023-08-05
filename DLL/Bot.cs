using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DLL;

public class Bot
{
    private readonly string _basePath = "https://eprog.importer.premiumtesh.nat.cu/api-data";
    private readonly string _channelPath = "/channel";
    private readonly string _eventPath = "/event";

    public Bot(string token)
    {
        var myBot = new TelegramBotClient(token);
        this.StartReceiver(myBot);
    }

    public async Task StartReceiver(ITelegramBotClient? botClient)
    {
        var cancellationTokenSource = new CancellationTokenSource().Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        if (botClient != null)
            await botClient.ReceiveAsync(OnMessage, ErrorMessage, receiverOptions, cancellationTokenSource);
    }

    public async Task OnMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { } message)
        {
            #region Commands Seccion

            switch (message.Text)
            {
                case "/channel_list":
                    await bot.SendTextMessageAsync(message.From!.Id, "Choose a channel",
                        replyMarkup: CallbackButtons.GetChannels(_basePath, _channelPath, 0, "sorts=name"),
                        cancellationToken: cancellationToken);
                    break;
                case "/start":
                    await bot.SendTextMessageAsync(message.From!.Id, "Welcome to the bot",
                        replyMarkup: CallbackButtons.Start,
                        cancellationToken: cancellationToken);
                    break;
            }

            #endregion
        }

        if (update.CallbackQuery is { } callbackQuery)
        {
            var args = callbackQuery.Data?.Split("-");

            #region InlineButtons Seccion

            switch (args?[0])
            {
                case "Channel List":
                    var reply = CallbackButtons.GetChannels(_basePath, _channelPath,
                        (args.Length > 1) ? int.Parse(args[1]) : 0, "sorts=name");
                    await bot.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                        cancellationToken: cancellationToken);
                    await bot.SendTextMessageAsync(callbackQuery.From.Id, "Choose a channel",
                        replyMarkup: reply,
                        cancellationToken: cancellationToken);
                    break;
                case "Channel":
                    reply = CallbackButtons.GetEventRange(_basePath, _eventPath,args[1], 0,
                        $"filters=channelName=={args[1]}","sorts=eventInitialDate");
                    await bot.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                        cancellationToken: cancellationToken);
                    await bot.SendTextMessageAsync(callbackQuery.From.Id, "Choose a date range",
                        replyMarkup: reply,
                        cancellationToken: cancellationToken);
                    break;
                case "Date List":
                    if (DateOnly.TryParse(args[2], out _))
                        throw new NotImplementedException();
                    else
                    {
                        reply = CallbackButtons.GetEventRange(_basePath, _eventPath,args[1], int.Parse(args[2]),
                            $"filters=channelName=={args[1]}","sorts=eventInitialDate");
                        await bot.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                            cancellationToken: cancellationToken);
                        await bot.SendTextMessageAsync(callbackQuery.From.Id, "Choose a date range",
                            replyMarkup: reply,
                            cancellationToken: cancellationToken);
                    }
                    break;
                case "Start":
                    await bot.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                        cancellationToken: cancellationToken);
                    await bot.SendTextMessageAsync(callbackQuery.From!.Id, "Welcome to the bot",
                        replyMarkup: CallbackButtons.Start,
                        cancellationToken: cancellationToken);
                    break;
            }

            #endregion
        }
    }

    public async Task ErrorMessage(ITelegramBotClient bot, Exception e, CancellationToken cancellationToken)
    {
        if (e is ApiRequestException requestException)
            await bot.SendTextMessageAsync(0, $"Error: {requestException.ErrorCode} - {requestException.Message}",
                cancellationToken: cancellationToken);
    }
}