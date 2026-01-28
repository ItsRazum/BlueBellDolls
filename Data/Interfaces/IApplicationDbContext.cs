using BlueBellDolls.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlueBellDolls.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ParentCat> Cats { get; }
        DbSet<Kitten> Kittens { get; }
        DbSet<Litter> Litters { get; }
        DbSet<EntityPhoto> Photos { get; }
        DbSet<TelegramPhoto> TelegramPhotos { get; }
        DbSet<CatColor> CatColors { get; }
        DbSet<BookingRequest> BookingRequests { get; }
        DbSet<FeedbackRequest> FeedbackRequests { get; }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class;

        public DatabaseFacade Database { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}