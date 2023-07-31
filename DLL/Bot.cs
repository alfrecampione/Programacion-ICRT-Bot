using System.Threading.Channels;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DLL;

public class Bot
{
    private readonly TelegramBotClient? _myBot;

    public Bot(string token)
    {
        _myBot = new TelegramBotClient(token);
        this.StartReceiver(_myBot);
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
            if (message.Text == "/channel_list")
                await bot.SendTextMessageAsync(message.From.Id, "Choose a channel",
                    replyMarkup: CallbackButtons.Channels,
                    cancellationToken: cancellationToken);
            if (message.Text == "/start")
                await bot.SendTextMessageAsync(message.From.Id, "Welcome to the bot",
                    replyMarkup: CallbackButtons.Start,
                    cancellationToken: cancellationToken);
            #endregion
        }

        if (update.CallbackQuery is {} callbackQuery)
        {
            #region InlineButtons Seccion
            if(callbackQuery.Data == "Channels List")
            {
                var messageSent = await bot.SendTextMessageAsync(callbackQuery.From.Id, "Choose a channel",
                    replyMarkup: CallbackButtons.Channels,
                    cancellationToken: cancellationToken);
                await bot.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                    cancellationToken: cancellationToken);
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