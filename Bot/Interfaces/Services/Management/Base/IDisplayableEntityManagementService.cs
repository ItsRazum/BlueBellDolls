using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management.Base
{
    public interface IDisplayableEntityManagementService<TEntity> : IEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity
    {
        Task<ServiceResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default);
        Task<ServiceResult<TEntity>> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default);
        Task<ServiceResult<TEntity>> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default);
        Task<ServiceResult<TEntity>> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token = default);
        Task<ServiceResult<TEntity>> SetDefaultPhotoToEntityAsync(int entityId, int photoId, CancellationToken token = default);
    }
}
