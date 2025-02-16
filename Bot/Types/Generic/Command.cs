using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Types.Generic
{
    internal class BotCommand<TCommandAdapter> : IBotCommand where TCommandAdapter : class, ICommandAdapter
    {

        private readonly Func<TCommandAdapter, CancellationToken, Task> _executeMethod;

        public BotCommand(Func<TCommandAdapter, CancellationToken, Task> executeMethod)
        {
            _executeMethod = executeMethod;
        }

        public virtual async Task<bool> ExecuteAsync(ICommandAdapter adapter, CancellationToken token)
        {
            if (adapter is TCommandAdapter commandAdapter)
            {
                await _executeMethod(commandAdapter, token);
                return true;
            }

            return false;
        }
    }
}
