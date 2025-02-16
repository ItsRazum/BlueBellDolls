using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IUpdateHandlerService
    {
        #region Methods

        Task HandleCommandAsync(Message m, bool containsArgs, CancellationToken token = default);

        Task HandleCallbackAsync(CallbackQuery c, bool containsArgs, CancellationToken token = default);

        Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token);

        Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token);

        #endregion
    }
}
