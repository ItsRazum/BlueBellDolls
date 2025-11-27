using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IParentCatManagementService : ICatManagementService<ParentCat>
    {
        Task<ManagementOperationResult<PagedResult<ParentCat>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default);
    }
}
