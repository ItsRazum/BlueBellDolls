using BlueBellDolls.Bot.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class MessageAdapter : ICommandAdapter
    {
        private readonly Message _message;

        public MessageAdapter(Message message)
        {
            _message = message;
        }

        public string Text => _message.Text ?? string.Empty;

        public int MessageId => _message.MessageId;

        public Chat Chat => _message.Chat;

        public User? From => _message.From;

        public Message? ReplyToMessage => _message.ReplyToMessage;

        public object Photos => _message.Photo ?? [];
    }
}
