using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management.Base
{
    public interface IEntityManagementService<TEntity> where TEntity : class, IEntity
    {
        Task<ServiceResult<TEntity>> GetEntityAsync(int entityId, CancellationToken token = default);
        Task<ServiceResult<TEntity>> AddNewEntityAsync(CancellationToken token = default);
        Task<ServiceResult> DeleteEntityAsync(int entityId, CancellationToken token = default);

        Task<ServiceResult<PagedResult<TEntity>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);

        Task<ServiceResult<TEntity>> UpdateEntityAsync(int modelId, Dictionary<string, string> properties, CancellationToken token = default);
    }
}