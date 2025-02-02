namespace EbookStore.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task<int> CompleteAsync();
    }
}
