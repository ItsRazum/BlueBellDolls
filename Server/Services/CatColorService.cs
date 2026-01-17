using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Records;
using BlueBellDolls.Server.Settings;
using BlueBellDolls.Server.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CatColor = BlueBellDolls.Common.Models.CatColor;

namespace BlueBellDolls.Server.Services
{
    public class CatColorService : DisplayableEntityServiceBase<CatColor, CatColorDetailDto>, ICatColorService
    {

        #region Fields

        private readonly IEntityFactory _entityFactory;
        private readonly ILogger<CatColorService> _logger;
        private readonly CatColorTree _catColorTree;
        private readonly string[] _catColorTreeMap;

        public CatColorService(
            IApplicationDbContext applicationDbContext,
            IWebHostEnvironment env,
            IEntityFactory entityFactory,
            ILogger<CatColorService> logger,
            IOptions<FileStorageSettings> fileStorageSettings,
            IOptions<EntitiesSettings> entitiesSettings) : base(env, applicationDbContext, fileStorageSettings, logger)
        {
            _logger = logger;
            _entityFactory = entityFactory;
            _catColorTree = entitiesSettings.Value.CatColors;
            _catColorTreeMap = _catColorTree.ToTreeMap();
        }

        #endregion

        #region Methods

        #region CRUD

        public async Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token)
        {
            try
            {
                if (!_catColorTreeMap.Contains(dto.Identifier))
                    return new(StatusCodes.Status400BadRequest, "Недопустимый идентификатор CatColor!");

                var entity = dto.ToEFModel();
                ApplicationDbContext.CatColors.Add(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status201Created, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(AddAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить новый CatColor!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось добавить новый CatColor!");
            }
        }

        public async Task<ServiceResult<PagedResult<CatColorListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.CatColors
                    .AsNoTracking()
                    .Where(c => admin || c.IsEnabled);

                var treeMapItems = _catColorTreeMap
                    .OrderBy(s => s)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                var items = await query
                    .Include(c => c.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .Where(c => treeMapItems.Contains(c.Identifier))
                    .ToListAsync(token);

                var zippedItems = treeMapItems.Select(identifier =>
                {
                    var entity = items.FirstOrDefault(c => c.Identifier == identifier);
                    entity ??= _entityFactory.CreateNewCatColor(identifier);

                    return entity.ToListDto();
                }).ToList();

                var totalItems = await query.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var result = new PagedResult<CatColorListDto>(zippedItems, pageNumber, pageSize, totalItems, totalPages);
                return new(StatusCodes.Status200OK, null, result);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(GetListAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу CatColor!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить страницу CatColor!");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default)
        {
            try
            {
                var result = await GetDetailEntityAsync(id, token);

                if (result == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                if (!result.IsEnabled && !admin)
                    return new(StatusCodes.Status403Forbidden, "Доступ к CatColor запрещен!");

                result.Photos = SortPhotosByDefault(result.Photos);

                return new(StatusCodes.Status200OK, null, result.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(GetAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить CatColor!");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> GetAsync(bool admin, string identifier, CancellationToken token = default)
        {
            try
            {
                var result = await ApplicationDbContext.CatColors
                    .Where(c => admin || c.IsEnabled)
                    .Include(c => c.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .FirstOrDefaultAsync(c => c.Identifier == identifier, token);

                if (result == null)
                {
                    if (!_catColorTreeMap.Contains(identifier))
                        return new(StatusCodes.Status400BadRequest, "Недопустимый идентификатор CatColor!");

                    result = _entityFactory.CreateNewCatColor(identifier);

                    await ApplicationDbContext.CatColors.AddAsync(result, token);
                    await ApplicationDbContext.SaveChangesAsync(token);
                }

                if (result.IsEnabled && !admin)
                    return new(StatusCodes.Status403Forbidden, "Доступ к CatColor запрещен!");

                result.Photos = SortPhotosByDefault(result.Photos);

                return new(StatusCodes.Status200OK, null, result.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(GetAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить CatColor {id}!", identifier);
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить CatColor!");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.CatColors.Include(k => k.Photos).FirstOrDefaultAsync(k => k.Id == id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                ApplicationDbContext.CatColors.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(DeleteAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось удалить CatColor!");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> UpdateAsync(int id, UpdateCatColorDto catColorDto, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.CatColors.FindAsync([id], token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                entity.ApplyUpdate(catColorDto);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(CatColorService), nameof(UpdateAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обновить CatColor!");
            }
        }

        public async Task<ServiceResult<CatColorTree>> GetColorTreeAsync(CancellationToken token = default)
        {
            return _catColorTree != null
                ? new(StatusCodes.Status200OK, null, _catColorTree)
                : new(StatusCodes.Status500InternalServerError, "Дерево цветов недоступно!");
        }

        #endregion

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override Func<CatColor, CatColorDetailDto> ModelToDtoFunc => (model) => model.ToDetailDto();

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath) || storagePath != "photos")
                return false;

            photosType = PhotosType.Photos;
            return true;
        }

        #endregion

    }
}
