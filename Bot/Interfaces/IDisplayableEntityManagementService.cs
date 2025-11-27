using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IDisplayableEntityManagementService<TEntity> : IEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity
    {
        Task<ManagementOperationResult> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default);
        Task<ManagementOperationResult> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default);
        Task<ManagementOperationResult> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token);
        Task<ManagementOperationResult> SetDefaultPhotoAsync(int entityId, int photoId, CancellationToken token = default);
    }
}
