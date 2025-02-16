using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ICommandAdapter
    {
        public Chat Chat { get; }
        public User? From { get; }
    }
}
