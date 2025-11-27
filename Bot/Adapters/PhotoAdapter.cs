using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class PhotoAdapter(PhotoSize photoSize, int messageId)
    {
        private readonly PhotoSize _photoSize = photoSize;

        public string FileId => _photoSize.FileId;

        public int MessageId { get; } = messageId;
    }
}
