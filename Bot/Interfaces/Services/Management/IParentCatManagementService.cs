using BlueBellDolls.Bot.Interfaces.Services.Management.Base;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface IParentCatManagementService : ICatManagementService<ParentCat>
    {
        Task<ServiceResult<PagedResult<ParentCat>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default);
    }
}
