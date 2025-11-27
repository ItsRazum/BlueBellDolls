using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ICatManagementService<TEntity> : IDisplayableEntityManagementService<TEntity> where TEntity : Cat
    {
        Task<ManagementOperationResult<TEntity>> UpdateColorAsync(int entityId, string color, CancellationToken token);
    }
}
