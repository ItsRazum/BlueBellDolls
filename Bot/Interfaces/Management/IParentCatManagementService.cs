using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Bot.Interfaces.Management.Base;

namespace BlueBellDolls.Bot.Interfaces.Management
{
    public interface IParentCatManagementService : ICatManagementService<ParentCat>
    {
        Task<ManagementOperationResult<PagedResult<ParentCat>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default);
    }
}
