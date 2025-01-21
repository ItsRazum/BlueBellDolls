using BlueBellDolls.Bot.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class CallbackQueryAdapter : ICommandContext
    {
        private readonly CallbackQuery _callbackQuery;

        public CallbackQueryAdapter(CallbackQuery callbackQuery)
        {
            _callbackQuery = callbackQuery;
        }

        public Chat Chat => _callbackQuery.Message != null
            ? _callbackQuery.Message.Chat
            : throw new NullReferenceException(nameof(_callbackQuery.Message.Chat));

        public User? From => _callbackQuery.From;
    }
}
