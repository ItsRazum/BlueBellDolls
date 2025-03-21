using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class MessageAdapter : ICommandAdapter
    {
        private readonly Message _message;

        public MessageAdapter(Message message, IEnumerable<PhotoAdapter>? photos = null)
        {
            _message = message;
            Photos = photos?.ToArray();
            if (_message.ReplyToMessage != null)
                ReplyToMessage = _message.ReplyToMessage.ToAdaper();
        }

        public string Text => _message.Text ?? string.Empty;

        public int MessageId => _message.MessageId;

        public Chat Chat => _message.Chat;

        public User? From => _message.From;

        public MessageAdapter? ReplyToMessage { get; }

        public PhotoAdapter[]? Photos { get; }

    }
}
