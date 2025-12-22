using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Types
{
    public abstract class CatServiceBase<TEntity>(
        IWebHostEnvironment env, 
        IApplicationDbContext applicationDbContext, 
        ILogger logger) : DisplayableEntityServiceBase<TEntity>(env, applicationDbContext, logger) where TEntity : Cat
    {

    }
}
