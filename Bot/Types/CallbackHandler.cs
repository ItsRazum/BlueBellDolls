using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Types
{
    public class CallbackHandler
    {
        private readonly Dictionary<string, Func<CallbackQueryAdapter, CancellationToken, Task>> _handlers = [];

        public IReadOnlyDictionary<string, Func<CallbackQueryAdapter, CancellationToken, Task>> Handlers => _handlers;

        protected IBotService BotService { get; }
        protected string CallbackArgsSeparator { get; }
        protected ICallbackDataProvider CallbackDataProvider { get; }

        public CallbackHandler(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider)
        {
            BotService = botService;
            CallbackArgsSeparator = botSettings.Value.CallbackDataSettings.ArgsSeparator;
            CallbackDataProvider = callbackDataProvider;
        }

        protected void AddCommandHandler(string command, Func<CallbackQueryAdapter, CancellationToken, Task> handler)
        {
            if (!_handlers.TryAdd(command, handler))
                throw new InvalidOperationException($"Обработчик команды '{command}' уже зарегистрирован!");
        }
    }
}
