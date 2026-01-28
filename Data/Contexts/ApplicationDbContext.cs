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
        public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
        public DbSet<FeedbackRequest> FeedbackRequests => Set<FeedbackRequest>();

        #endregion

        #region DbContext implementation

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        #endregion
    }
}
