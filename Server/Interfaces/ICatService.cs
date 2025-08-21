using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Server.Interfaces
{
    internal interface ICatService
    {
        Task<IEnumerable<ParentCat>> GetActiveCatsByGenderAsync(bool isMale, CancellationToken token = default);
        Task<IEnumerable<Litter>> GetActiveLittersAsync(CancellationToken token = default);
    }
}
