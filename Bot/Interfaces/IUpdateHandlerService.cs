using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IUpdateHandlerService : IUpdateHandler
    {
        #region Methods

        Task HandleCommandAsync(Message m, bool containsArgs, CancellationToken token = default);

        Task HandleCallbackAsync(CallbackQuery c, bool containsArgs, CancellationToken token = default);

        #endregion
    }
}
