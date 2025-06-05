using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IManagementService
    {
        Task<ManagementOperationResult<TEntity>> AddNewEntityAsync<TEntity>(CancellationToken token = default) where TEntity : class, IDisplayableEntity, new();
        Task<ManagementOperationResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token);
        Task<ManagementOperationResult> DeleteEntityAsync<TEntity>(int entityId, CancellationToken token) where TEntity : class, IDisplayableEntity;
        Task<ManagementOperationResult> DeleteEntityPhotosAsync<TEntity>(int entityId, IEnumerable<int> photoIndexes, PhotosManagementMode photosManagementMode, CancellationToken token) where TEntity : IDisplayableEntity;
        Task<ManagementOperationResult<TEntity>> SetDefaultPhotoForEntityAsync<TEntity>(int entityId, int photoIndex, CancellationToken token) where TEntity : class, IDisplayableEntity;
        Task<ManagementOperationResult<Litter>> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token);
        Task<ManagementOperationResult<TEntity>> UpdateEntityByReplyAsync<TEntity>(int modelId, Dictionary<string, string> properties, CancellationToken token = default) where TEntity : class, IDisplayableEntity;
        Task<ManagementOperationResult<TEntity>> UpdateEntityColorAsync<TEntity>(int entityId, string color, CancellationToken token) where TEntity : Cat;
        Task<ManagementOperationResult<StructWrapper<(int parentCatsCount, int littersCount, int kittensCount)>>> ActivateEntitiesAsync(CancellationToken token);
        Task<ManagementOperationResult<TEntity>> ToggleEntityVisibilityAsync<TEntity>(int entityId, CancellationToken token) where TEntity : class, IDisplayableEntity;
    }
}