using EbookStore.Domain.Common.EbookStore.Domain.Common;
using EbookStore.Infrastructure.Repositories.Interfaces;
using EbookStore.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EbookStore.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EbookStoreDbContext context;
        private readonly DbSet<T> dbSet;
        private readonly ILogger<GenericRepository<T>> logger;

        public GenericRepository(EbookStoreDbContext context, ILogger<GenericRepository<T>> logger)
        {
            this.context = context;
            dbSet = context.Set<T>();
            this.logger = logger;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                logger.LogInformation("Getting all entities of type {Type}", typeof(T).Name);

                IQueryable<T> query = dbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
                {
                    query = query.Where(e => !((BaseEntity)(object)e).IsDeleted);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting all entities of type {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                logger.LogInformation("Getting entity of type {Type} with id {Id}", typeof(T).Name, id);

                IQueryable<T> query = dbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting entity of type {Type} with id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                logger.LogInformation("Adding entity of type {Type}", typeof(T).Name);
                await dbSet.AddAsync(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while adding entity of type {Type}", typeof(T).Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                logger.LogInformation("Updating entity of type {Type}", typeof(T).Name);
                dbSet.Update(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating entity of type {Type}", typeof(T).Name);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                logger.LogInformation("Deleting entity of type {Type} with id {Id}", typeof(T).Name, id);
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    logger.LogWarning("Entity of type {Type} with id {Id} not found", typeof(T).Name, id);
                    return false;
                }

                dbSet.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting entity of type {Type} with id {Id}", typeof(T).Name, id);
                return false;
            }
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            try
            {
                logger.LogInformation("Soft deleting entity of type {Type} with id {Id}", typeof(T).Name, id);
                var entity = await dbSet.FindAsync(id);
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.IsDeleted = true;
                    baseEntity.DeletedAt = DateTime.UtcNow;
                    dbSet.Update(entity);
                    await context.SaveChangesAsync();
                    return true;
                }

                logger.LogWarning("Entity of type {Type} with id {Id} is not a BaseEntity", typeof(T).Name, id);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while soft deleting entity of type {Type} with id {Id}", typeof(T).Name, id);
                return false;
            }
        }
    }
}