using EbookStore.Infrastructure.Repositories.Interfaces;
using EbookStore.Persistence.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace EbookStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EbookStoreDbContext context;
        private readonly ILogger<UnitOfWork> logger;
        private readonly ILoggerFactory loggerFactory;
        private bool disposed = false;
        private readonly ConcurrentDictionary<Type, object> repositories;

        public UnitOfWork(EbookStoreDbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
        {
            this.context = context;
            this.logger = logger;
            this.loggerFactory = loggerFactory;
            repositories = new ConcurrentDictionary<Type, object>();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return (IGenericRepository<T>)repositories.GetOrAdd(typeof(T), _ =>
                new GenericRepository<T>(context, loggerFactory.CreateLogger<GenericRepository<T>>()));
        }

        public async Task<int> CompleteAsync()
        {
            try
            {
                logger.LogInformation("Saving all changes to the database!");
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while saving changes to the database.");
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    logger.LogInformation("Disposing the UnitOfWork and DbContext.");
                    context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}