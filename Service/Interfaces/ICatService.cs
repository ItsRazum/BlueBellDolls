using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Service.Interfaces
{
    internal interface ICatService
    {
        Task<IEnumerable<ParentCat>> GetActiveCatsByGenderAsync(bool isMale, CancellationToken token = default);
        Task<IEnumerable<Litter>> GetActiveLittersAsync(CancellationToken token = default);
    }
}
