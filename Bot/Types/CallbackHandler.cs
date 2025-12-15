using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Types
{
    public abstract class CallbackHandler(
        IBotService botService,
        IOptions<BotSettings> botSettings,
        ICallbackDataProvider callbackDataProvider)
    {
        private readonly Dictionary<string, Func<CallbackQueryAdapter, CancellationToken, Task>> _handlers = [];

        public IReadOnlyDictionary<string, Func<CallbackQueryAdapter, CancellationToken, Task>> Handlers => _handlers;

        protected IBotService BotService { get; } = botService;
        protected string CallbackArgsSeparator { get; } = botSettings.Value.CallbackDataSettings.ArgsSeparator;
        protected ICallbackDataProvider CallbackDataProvider { get; } = callbackDataProvider;

        protected void AddCommandHandler(string command, Func<CallbackQueryAdapter, CancellationToken, Task> handler)
        {
            if (!_handlers.TryAdd(command, handler))
                throw new InvalidOperationException($"Обработчик команды '{command}' уже зарегистрирован!");
        }
    }
}
