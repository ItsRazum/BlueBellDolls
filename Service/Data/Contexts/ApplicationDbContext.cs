using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Service.Data.Contexts
{
    internal class ApplicationDbContext : DbContext
    {

        #region DbSets

        public DbSet<ParentCat> Cats => Set<ParentCat>();
        public DbSet<Kitten> Kittens => Set<Kitten>();
        public DbSet<Litter> Litters => Set<Litter>();

        #endregion

        #region Constructor

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region DbContext implementation

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        #endregion
    }
}
