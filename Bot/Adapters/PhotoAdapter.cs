using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Adapters
{
    public class PhotoAdapter
    {
        private PhotoSize _photoSize;

        public PhotoAdapter(PhotoSize photoSize, int messageId)
        {
            _photoSize = photoSize;
            MessageId = messageId;
        }

        public string FileId => _photoSize.FileId;

        public int MessageId { get; }
    }
}
