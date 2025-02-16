using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IEntityUpdateService
    {
        Task<bool> HandleUpdateEntityByReplyAsync<TEntity>(
            MessageAdapter m,
            string modelName,
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token) where TEntity : class, IDisplayableEntity;
    }
}
