using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Data.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        #region DbSets

        public DbSet<ParentCat> Cats => Set<ParentCat>();
        public DbSet<Kitten> Kittens => Set<Kitten>();
        public DbSet<Litter> Litters => Set<Litter>();

        #endregion
        #region Constructor

        #endregion

        #region DbContext implementation

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries<EntityBase>()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entities)
            {
                var hasNonIsEnabledChanges = entry.Properties
                    .Any(p => p.Metadata.Name != nameof(EntityBase.IsEnabled) && p.IsModified);

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
