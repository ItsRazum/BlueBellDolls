using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    internal interface ICommandContext
    {
        public Chat Chat { get; }
        public User? From { get; }
    }
}
