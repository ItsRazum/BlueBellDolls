using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Data.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {

        #region DbSets

        public DbSet<ParentCat> Cats => Set<ParentCat>();
        public DbSet<Kitten> Kittens => Set<Kitten>();
        public DbSet<Litter> Litters => Set<Litter>();
        public DbSet<EntityPhoto> Photos => Set<EntityPhoto>();
        public DbSet<TelegramPhoto> TelegramPhotos => Set<TelegramPhoto>();
        public DbSet<CatColor> CatColors => Set<CatColor>();

        #endregion

        #region DbContext implementation

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries< DisplayableEntityBase> ()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entities)
            {
                var hasNonIsEnabledChanges = entry.Properties
                    .Any(p => p.Metadata.Name != nameof(DisplayableEntityBase.IsEnabled) && p.IsModified);

                if (hasNonIsEnabledChanges)
                {
                    if (!entry.Property(x => x.IsEnabled).IsModified)
                    {
                        entry.Entity.IsEnabled = false;
                    }
                }
            }

            return base.SaveChanges();
        }

        #endregion
    }
}
