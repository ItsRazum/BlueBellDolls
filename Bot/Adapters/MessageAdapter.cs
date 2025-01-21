using BlueBellDolls.Bot.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class MessageAdapter : ICommandContext
    {
        private readonly Message _message;

        public MessageAdapter(Message message)
        {
            _message = message;
        }

        public Chat Chat => _message.Chat;

        public User? From => _message.From;

        public Message? ReplyToMessage => _message.ReplyToMessage;
    }
}
