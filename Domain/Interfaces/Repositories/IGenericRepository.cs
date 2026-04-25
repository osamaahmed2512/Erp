using Domain.Interfaces.Specification;
using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = true);
        Task<T> GetByIdSpecAsync(ISpecification<T> Spec);
        Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null);
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec);
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountWithSpec(ISpecification<T> Spec);
        Task<int> CountAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(ISpecification<T> spec);
        Task<TResult?> GetSingleProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null);
        Task<List<T>> FromSqlAsync(string sql, params object[] parameters);
    }
}
