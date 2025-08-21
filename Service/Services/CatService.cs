using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;

namespace BlueBellDolls.Server.Services
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

        public async Task<IEnumerable<ParentCat>> GetActiveCatsByGenderAsync(bool isMale, CancellationToken token = default)
        {
            return await _unitOfWork.GetRepository<ParentCat>().GetAllAsync(c => c.IsMale == isMale && c.IsEnabled, token);
        }

        public async Task<IEnumerable<Litter>> GetActiveLittersAsync(CancellationToken token = default)
        {
            return await _unitOfWork.GetRepository<Litter>().GetAllAsync(l => l.IsActive, token);
        }

        #endregion

    }
}
