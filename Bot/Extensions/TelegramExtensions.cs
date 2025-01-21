using BlueBellDolls.Bot.Adapters;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Extensions
{
    public static class TelegramExtensions
    {
        public static MessageAdapter ToAdaper(this Message message)
        {
            return new MessageAdapter(message);
        }

        public static CallbackQueryAdapter ToAdaper(this CallbackQuery message) 
        { 
            return new CallbackQueryAdapter(message); 
        }
    }
}
