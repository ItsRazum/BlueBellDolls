using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces.Types
{
    public interface ICommandAdapter
    {
        public Chat Chat { get; }
        public User? From { get; }
    }
}
