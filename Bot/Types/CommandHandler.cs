using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Types
{
    public abstract class CommandHandler
    {
        private readonly Dictionary<string, Func<MessageAdapter, CancellationToken, Task>> _handlers = [];

        public IReadOnlyDictionary<string, Func<MessageAdapter, CancellationToken, Task>> Handlers => _handlers;

        protected IBotService BotService { get; }

        public CommandHandler(
            IBotService botService)
        {
            BotService = botService;
        }

        protected void AddCommandHandler(string command, Func<MessageAdapter, CancellationToken, Task> handler)
        {
            if (!_handlers.TryAdd(command, handler))
                throw new InvalidOperationException($"Обработчик команды '{command}' уже зарегистрирован!");
        }
    }
}
