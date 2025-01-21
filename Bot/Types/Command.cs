using BlueBellDolls.Bot.Interfaces;

namespace BlueBellDolls.Bot.Types
{
    internal class Command<TCommandContext> where TCommandContext : class, ICommandContext
    {
        private readonly Func<TCommandContext, CancellationToken, Task> _executeMethod;

        public Command(Func<TCommandContext, CancellationToken, Task> executeMethod)
        {
            _executeMethod = executeMethod;
        }

        public virtual async Task<bool> ExecuteAsync(TCommandContext c, CancellationToken token) 
        {
            if (c.From?.Id is 901152811 or 821067036)
            {
                await _executeMethod(c, token);
                return true;
            }

            return false;
        }
    }
}
