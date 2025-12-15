using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Bot.Factories
{
    public class ManagementServicesFactory(IServiceProvider serviceProvider) : IManagementServicesFactory
    {

        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IEntityManagementService<TEntity> GetEntityManagementService<TEntity>() where TEntity : class, IEntity
        {
            return _serviceProvider.GetRequiredService<IEntityManagementService<TEntity>>();
        }

        public IDisplayableEntityManagementService<TEntity> GetDisplayableEntityManagementService<TEntity>() where TEntity : class, IDisplayableEntity
        {
            return _serviceProvider.GetRequiredService<IDisplayableEntityManagementService<TEntity>>();
        }

        public ICatManagementService<TEntity> GetCatManagementService<TEntity>() where TEntity : Cat
        {
            return _serviceProvider.GetRequiredService<ICatManagementService<TEntity>>();
        }

        public ILitterManagementService GetLitterManagementService()
        {
            return _serviceProvider.GetRequiredService<ILitterManagementService>();
        }

        public IParentCatManagementService GetParentCatManagementService()
        {
            return _serviceProvider.GetRequiredService<IParentCatManagementService>();
        }
    }
}
