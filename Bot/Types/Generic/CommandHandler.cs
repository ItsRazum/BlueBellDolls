using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Types.Generic
{
    public abstract class CommandHandler<TCommandAdapter> where TCommandAdapter : ICommandAdapter
    {
        public Dictionary<string, Func<TCommandAdapter, CancellationToken, Task>> Handlers { get; protected set; }
        protected IBotService BotService { get; }

        public CommandHandler(IBotService botService)
        {
            BotService = botService;
            Handlers = [];
        }
    }
}
