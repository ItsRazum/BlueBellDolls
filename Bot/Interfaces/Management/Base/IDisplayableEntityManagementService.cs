using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Management.Base
{
    public interface IDisplayableEntityManagementService<TEntity> : IEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity
    {
        Task<ManagementOperationResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default);
        Task<ManagementOperationResult<TEntity>> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default);
        Task<ManagementOperationResult<TEntity>> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default);
        Task<ManagementOperationResult<TEntity>> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token = default);
        Task<ManagementOperationResult<TEntity>> SetDefaultPhotoToEntityAsync(int entityId, int photoId, CancellationToken token = default);
    }
}
