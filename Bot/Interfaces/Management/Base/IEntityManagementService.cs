using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Management.Base
{
    public interface IEntityManagementService<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> GetEntityAsync(int entityId, CancellationToken token = default);
        Task<ManagementOperationResult<TEntity>> AddNewEntityAsync(CancellationToken token = default);
        Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token = default);

        Task<ManagementOperationResult<PagedResult<TEntity>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);

        Task<ManagementOperationResult<TEntity>> UpdateEntityAsync(TEntity entity, CancellationToken token = default);

        Task<ManagementOperationResult<TEntity>> UpdateEntityByReplyAsync(int modelId, Dictionary<string, string> properties, CancellationToken token = default);
    }
}