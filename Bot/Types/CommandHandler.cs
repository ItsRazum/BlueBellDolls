using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Types
{
    public abstract class CommandHandler(IBotService botService)
    {
        private readonly Dictionary<string, Func<MessageAdapter, CancellationToken, Task>> _handlers = [];

        public IReadOnlyDictionary<string, Func<MessageAdapter, CancellationToken, Task>> Handlers => _handlers;

        protected IBotService BotService { get; } = botService;

        protected void AddCommandHandler(string command, Func<MessageAdapter, CancellationToken, Task> handler)
        {
            if (!_handlers.TryAdd(command, handler))
                throw new InvalidOperationException($"Обработчик команды '{command}' уже зарегистрирован!");
        }
    }
}
