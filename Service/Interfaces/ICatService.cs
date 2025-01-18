using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Service.Interfaces
{
    internal interface ICatService
    {
        Task<IEnumerable<ParentCat>> GetCatsByGenderAsync(bool isMale, CancellationToken token = default);
        Task<IEnumerable<Litter>> GetLittersAsync(CancellationToken token = default);
        Task<bool> AddNewEntityAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class, IEntity;
        Task<bool> UpdateEntityAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class, IEntity;
    }
}
