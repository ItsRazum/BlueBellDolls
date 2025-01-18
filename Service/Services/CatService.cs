using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Service.Interfaces;

namespace BlueBellDolls.Service.Services
{
    internal sealed class CatService : ICatService
    {

        #region Fields

        private readonly ILogger<CatService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public CatService(ILogger<CatService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<ParentCat>> GetCatsByGenderAsync(bool isMale, CancellationToken token = default)
        {
            return (await _unitOfWork.GetRepository<ParentCat>().GetAllAsync(token)).Where(c => c.IsMale == isMale);
        }

        public async Task<IEnumerable<Litter>> GetLittersAsync(CancellationToken token = default)
        {
            return await _unitOfWork.GetRepository<Litter>().GetAllAsync(token);
        }

        public async Task<bool> AddNewEntityAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class, IEntity
        {
            try
            {
                await _unitOfWork.GetRepository<TEntity>().AddAsync(entity, token);
                await _unitOfWork.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in CatService.AddNewEntityAsync<{entity}>()", typeof(TEntity).Name);
                return false;
            }
        }

        public async Task<bool> UpdateEntityAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class, IEntity
        {
            try
            {
                await _unitOfWork.GetRepository<TEntity>().UpdateAsync(entity, token);
                await _unitOfWork.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in CatService.AddNewEntityAsync<{entity}>()", typeof(TEntity).Name);
                return false;
            }
        }

        #endregion

    }
}
