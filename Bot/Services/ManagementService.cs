using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Services
{
    public class ManagementService(
        IDatabaseService databaseService,
        IMessagesProvider messagesProvider,
        ILogger<ManagementService> logger,
        IEntityHelperService entityHelperService,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IOptions<EntitySettings> entotyOptions) : IManagementService
    {

        #region Fields

        private readonly IDatabaseService _databaseService = databaseService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<ManagementService> _logger = logger;
        private readonly IEntityHelperService _entityHelperService = entityHelperService;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly EntitySettings _entitySettings = entotyOptions.Value;

        #endregion

        #region IManagementService implementation

        #region Add/delete entities

        public async Task<ManagementOperationResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token)
        {
            var kitten = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var litterRepo = unit.GetRepository<Litter>();

                var litter = await litterRepo.GetByIdAsync(litterId, ct);

                if (litter == null)
                    return null;

                var kitten = new Kitten()
                {
                    BirthDay = litter.BirthDay
                };
                litter.Kittens.Add(kitten);
                await unit.SaveChangesAsync(token);

                return kitten;

            }, token);

            return kitten is null ? new(false, _messagesProvider.CreateEntityNotFoundMessage()) : new(true, null, kitten);

        }

        public async Task<ManagementOperationResult<TEntity>> AddNewEntityAsync<TEntity>(CancellationToken token = default) where TEntity : class, IDisplayableEntity, new()
        {
            try
            {
                var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var result = new TEntity();
                    await unit.GetRepository<TEntity>().AddAsync(result, ct);
                    await unit.SaveChangesAsync(ct);
                    return result;
                }, token);

                return new(true, null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(AddNewEntityAsync), typeof(TEntity).Name);
                return new(false, "Не удалось добавить сущность: " + ex.Message);
            }
        }

        public async Task<ManagementOperationResult> DeleteEntityAsync<TEntity>(int entityId, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            try
            {
                await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    if (typeof(TEntity) == typeof(ParentCat))
                    {
                        var parentCatRepo = unit.GetRepository<ParentCat>();
                        var targetParentCat = await parentCatRepo.GetByIdAsync(entityId, ct);

                        if (targetParentCat != null)
                        {
                            var litterRepo = unit.GetRepository<Litter>();

                            IEnumerable<Litter> entities = targetParentCat.IsMale
                                ? await litterRepo.GetAllAsync(l => l.FatherCatId == targetParentCat.Id, ct, l => l.FatherCat)
                                : await litterRepo.GetAllAsync(l => l.MotherCatId == targetParentCat.Id, ct, l => l.MotherCat);

                            await parentCatRepo.DeleteByIdAsync(entityId, ct);
                            await unit.SaveChangesAsync(ct);
                        }
                    }
                    else
                    {
                        var repo = unit.GetRepository<TEntity>();

                        if (await repo.DeleteByIdAsync(entityId, ct))
                            await unit.SaveChangesAsync(ct);
                    }
                }, token);

                return new(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(DeleteEntityAsync), typeof(TEntity).Name);
                return new(false, "Не удалось удалить сущность: " + ex.Message);
            }

        }

        #endregion

        #region Update entities

        public async Task<ManagementOperationResult<TEntity>> UpdateEntityColorAsync<TEntity>(int entityId, string color, CancellationToken token) where TEntity : Cat
        {
            try
            {
                var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entity == null)
                        return new ManagementOperationResult<TEntity>(false, _messagesProvider.CreateEntityNotFoundMessage());

                    entity.Color = color;
                    await unit.SaveChangesAsync(ct);
                    return new ManagementOperationResult<TEntity>(true, null, entity);
                }, token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(UpdateEntityColorAsync), typeof(TEntity).Name);
                return new(false, "Не удалось установить окрас сущности: " + ex.Message);
            }
        }

        public async Task<ManagementOperationResult<Litter>> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token)
        {
            try
            {
                var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var parentCat = await unit.GetRepository<ParentCat>().GetByIdAsync(parentCatId, ct);
                    var litter = await unit.GetRepository<Litter>().GetByIdAsync(litterId, ct);

                    if (litter == null)
                        return new ManagementOperationResult<Litter>(false, _messagesProvider.CreateEntityNotFoundMessage(typeof(Litter), litterId));

                    if (parentCat == null)
                        return new ManagementOperationResult<Litter>(false, _messagesProvider.CreateEntityNotFoundMessage(typeof(ParentCat), parentCatId));


                    if (parentCat.IsMale)
                        litter.FatherCat = parentCat;
                    else
                        litter.MotherCat = parentCat;

                    await unit.SaveChangesAsync(ct);
                    return new ManagementOperationResult<Litter>(true, null, litter);
                }, token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(ManagementService), nameof(SetParentCatForLitterAsync));
                return new ManagementOperationResult<Litter>(false, "Не удалось установить родителя для помета: " + ex.Message);
            }
        }

        public async Task<ManagementOperationResult<TEntity>> UpdateEntityByReplyAsync<TEntity>(
            int modelId,
            Dictionary<string, string>
            properties,
            CancellationToken token = default)
            where TEntity : class, IDisplayableEntity
        {
            try
            {

                var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(modelId, token);

                bool? originalIsMale = null;
                if (entity is ParentCat originalParent)
                {
                    originalIsMale = originalParent.IsMale;
                }

                var success = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var editedEntity = await unit.GetRepository<TEntity>().GetByIdAsync(modelId, ct);
                    if (editedEntity != null)
                    {
                        foreach (var (propertyName, value) in properties)
                            if (!_entityFormService.UpdateProperty(editedEntity, propertyName, value))
                                return false;

                        if (editedEntity is ParentCat updatedParent && originalIsMale.HasValue)
                        {
                            if (originalIsMale.Value != updatedParent.IsMale)
                            {
                                var litterRepo = unit.GetRepository<Litter>();

                                var relatedLitters = await litterRepo
                                    .GetAllAsync(l => l.MotherCatId == updatedParent.Id
                                             || l.FatherCatId == updatedParent.Id, token);

                                foreach (var litter in relatedLitters)
                                {
                                    if (updatedParent.IsMale)
                                    {
                                        if (litter.MotherCatId == updatedParent.Id)
                                            litter.MotherCatId = null;
                                    }
                                    else
                                    {
                                        if (litter.FatherCatId == updatedParent.Id)
                                            litter.FatherCatId = null;
                                    }
                                }
                            }
                        }

                        await unit.SaveChangesAsync(token);
                        entity = editedEntity;
                        return true;
                    }
                    return false;
                }, token);

                return success
                    ? new ManagementOperationResult<TEntity>(true, Result: entity)
                    : new ManagementOperationResult<TEntity>(false, "Произошла неизвестная ошибка");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(UpdateEntityByReplyAsync), typeof(TEntity).Name);
                return new(false, "Не удалось обновить сущность: " + ex.Message);
            }
        }

        #endregion

        #region Photos

        public async Task<ManagementOperationResult<IDisplayableEntity>> AddPhotosToEntity<TEntity>(
            PhotoAdapter[] photos,
            int entityId, 
            CancellationToken token) where TEntity : IDisplayableEntity
        {
            try
            {
                var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);
                if (entity == null)
                    return new(false, _messagesProvider.CreateEntityNotFoundMessage());

                if (entity.Photos.Count + photos.Length > _entitySettings.MaxPhotos[entity.GetType().Name])
                    return new(false, _messagesProvider.CreatePhotosLimitReachedMessage(entity));

                var base64Photos = await _photosDownloaderService.DownloadAndConvertPhotosToBase64(photos, token);
                if (base64Photos.Count == 0)
                    return new(false, null, entity);

                return await _databaseService.ExecuteDbOperationAsync<ManagementOperationResult<IDisplayableEntity>>(async (unit, ct) =>
                {
                    var entityFromDb = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entityFromDb == null) return new(false, _messagesProvider.CreateEntityNotFoundMessage());

                    if (entityFromDb.Photos.Count + base64Photos.Count > _entitySettings.MaxPhotos[entity.GetType().Name])
                        return new(false, _messagesProvider.CreatePhotosLimitReachedMessage(entityFromDb));

                    foreach (var photo in base64Photos)
                        entityFromDb.Photos.Add(photo.Key, photo.Value);

                    await unit.SaveChangesAsync(ct);
                    return new(true, null, entityFromDb);

                }, token);
            }
            catch (Exception ex)
            {
                return new(false, $"Не удалось добавить фото сущности: " + ex.Message);
            }
        }

        public async Task<ManagementOperationResult> DeleteEntityPhotosAsync<TEntity>(int entityId, IEnumerable<int> photoIndexes, PhotosManagementMode photosManagementMode, CancellationToken token) where TEntity : IDisplayableEntity
        {
            try
            {
                var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entity != null)
                    {
                        Dictionary<string, string> photos;
                        if (entity is ParentCat parentCat)
                        {
                            photos = photosManagementMode switch
                            {
                                PhotosManagementMode.Photos => entity.Photos,
                                PhotosManagementMode.Titles => parentCat.Titles,
                                PhotosManagementMode.GenTests => parentCat.GeneticTests,
                                _ => []
                            };
                        }
                        else
                        {
                            if (photosManagementMode != PhotosManagementMode.Photos)
                                throw new InvalidOperationException($"Сущность {entity.GetType().Name} не поддерживает удаление {photosManagementMode}.");

                            photos = entity.Photos;
                        }

                        var keysToRemove = photoIndexes
                        .Select(x => photos.Keys.ElementAt(x))
                        .ToList();

                        foreach (var photoKey in keysToRemove)
                            photos.Remove(photoKey);

                        await unit.SaveChangesAsync(token);

                        return new ManagementOperationResult(true);
                    }

                    return new ManagementOperationResult(false, _messagesProvider.CreateEntityNotFoundMessage());
                }, token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(DeleteEntityPhotosAsync), typeof(TEntity).Name);
                return new(false, "Не удалось удалить фото у сущности: " + ex.Message);
            }
        }

        public async Task<ManagementOperationResult<TEntity>> SetDefaultPhotoForEntityAsync<TEntity>(int entityId, int photoIndex, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            try
            {
                var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                {
                    var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entity == null)
                        return new ManagementOperationResult<TEntity>(false, _messagesProvider.CreateEntityNotFoundMessage());

                    var photosList = entity.Photos.ToList();
                    var photo = photosList[photoIndex];
                    photosList.RemoveAt(photoIndex);
                    photosList.Insert(0, photo);

                    entity.Photos = photosList.ToDictionary(x => x.Key, x => x.Value);
                    await unit.SaveChangesAsync(ct);
                    return new ManagementOperationResult<TEntity>(true, null, entity);
                }, token);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(UpdateEntityColorAsync), typeof(TEntity).Name);
                return new(false, "Не удалось установить заглавное фото для сущности: " + ex.Message);
            }

        }

        #endregion

        #region Entitites visibility

        public async Task<ManagementOperationResult<StructWrapper<(int parentCatsCount, int littersCount, int kittensCount)>>> ActivateEntitiesAsync(CancellationToken token)
        {
            try
            {
                return await _databaseService.ExecuteDbOperationAsync
                    <ManagementOperationResult<StructWrapper<(int parentCatsCount, int littersCount, int kittensCount)>>>
                    (async (unit, token) =>
                    {
                        var parentCatsCount = await ActivateAsync<ParentCat>(unit, token);
                        var littersCount = await ActivateAsync<Litter>(unit, token);
                        var kittensCount = await ActivateAsync<Kitten>(unit, token);

                        await unit.SaveChangesAsync(token);
                        return new(true, null, new((parentCatsCount, littersCount, kittensCount)));
                    }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(ManagementService), nameof(ActivateEntitiesAsync));
                return new(false, "Не удалось сохранить изменения: " + ex.Message);
            }

            static async Task<int> ActivateAsync<TEntity>(IUnitOfWork unit, CancellationToken token) where TEntity : IEntity
            {
                var enitities = await unit.GetRepository<TEntity>().GetAllAsync(e => !e.IsEnabled, token);
                foreach (var entity in enitities)
                    entity.IsEnabled = true;

                return enitities.Count();
            }
        }

        public async Task<ManagementOperationResult<TEntity>> ToggleEntityVisibilityAsync<TEntity>(int entityId, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            try
            {
                return await _databaseService.ExecuteDbOperationAsync(async (unit, token) =>
                {
                    var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, token);
                    if (entity != null)
                    {
                        entity.IsEnabled = !entity.IsEnabled;

                        await unit.SaveChangesAsync(token);
                        return new ManagementOperationResult<TEntity>(true, null, entity);
                    }

                    return new ManagementOperationResult<TEntity>(false, _messagesProvider.CreateEntityNotFoundMessage());
                }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}<{TEntity}>(): Произошла необработанная ошибка", nameof(ManagementService), nameof(ToggleEntityVisibilityAsync), typeof(TEntity).Name);
                return new(false, "Не удалось изменить состояние активности сущности: " + ex.Message);
            }
        }

        #endregion

        #endregion

    }
}
