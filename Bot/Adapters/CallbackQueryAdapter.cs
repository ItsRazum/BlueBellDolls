using BlueBellDolls.Bot.Interfaces.Types;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class CallbackQueryAdapter : ICommandAdapter
    {
        private readonly CallbackQuery _callbackQuery;

        public CallbackQueryAdapter(CallbackQuery callbackQuery)
        {
            _callbackQuery = callbackQuery;
        }

        public Chat Chat => _callbackQuery.Message != null
            ? _callbackQuery.Message.Chat
            : throw new NullReferenceException(nameof(_callbackQuery.Message.Chat));

        public string CallbackId => _callbackQuery.Id;

        public User? From => _callbackQuery.From;

        public string CallbackData => _callbackQuery.Data ?? string.Empty;

        public string MessageText => _callbackQuery.Message?.Text ?? string.Empty;

        public int MessageId => _callbackQuery.Message?.MessageId ?? 0;
    }
}
