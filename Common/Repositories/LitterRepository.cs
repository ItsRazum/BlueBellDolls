using BlueBellDolls.Common.Data.Contexts;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Common.Repositories
{
    public class LitterRepository(ApplicationDbContext dbContext) : EntityRepository<Litter>(dbContext)
    {
        public override async Task<Litter?> GetByIdAsync(int id, CancellationToken token)
        {
            return await DbSet
                .Include(l => l.FatherCat)
                .Include(l => l.MotherCat)
                .Include(l => l.Kittens)
                .FirstOrDefaultAsync(l => l.Id == id, token);
        }
    }
}
