using Domain.Interfaces.Repositories;
using System.Data;
namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task<bool> CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        Task<List<TResult>> ExecuteSqlInterpolatedAsync<TResult>(FormattableString sql) where TResult : class;

    }
}
