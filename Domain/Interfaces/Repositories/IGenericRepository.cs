using Domain.Interfaces.Specification;
using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<T> GetByIdSpecAsync(ISpecification<T> Spec, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec);
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountWithSpec(ISpecification<T> Spec, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<TResult?> GetSingleProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null, CancellationToken cancellationToken = default);
        Task<List<T>> FromSqlAsync(string sql, params object[] parameters);
    }
}
