using Microsoft.EntityFrameworkCore;
using ServiceManagerApp.Entities;

namespace ServiceManagerApp.Repositories
{
    public abstract class BaseRepository<T>(AppDbContext context) where T : BaseEntity
    {
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public virtual async Task CreateAsync(T entity, bool save = true)
        {
            await context.AddAsync(entity);
            if (save) await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity, bool save = true)
        {
            context.Update(entity);
            if (save) await context.SaveChangesAsync();
        }

        public virtual async Task<T?> GetAsync(long id)
        {
            return await DbSet
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
