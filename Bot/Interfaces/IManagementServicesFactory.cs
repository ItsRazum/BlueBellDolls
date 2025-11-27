using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IManagementServicesFactory
    {
        IEntityManagementService<TEntity> GetEntityManagementService<TEntity>() where TEntity : class, IEntity;
        IDisplayableEntityManagementService<TEntity> GetDisplayableEntityManagementService<TEntity>() where TEntity : class, IDisplayableEntity;
        ICatManagementService<TEntity> GetCatManagementService<TEntity>() where TEntity : Cat;
        ILitterManagementService GetLitterManagementService();
        IParentCatManagementService GetParentCatManagementService();
    }
}