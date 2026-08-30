using System.Data;
using System.Linq.Expressions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Specification;
using Domain.Interfaces.UnitOfWork;

namespace Application.Tests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public int SaveCount { get; private set; }

    public FakeUnitOfWork Seed<T>(params T[] entities) where T : class
    {
        _repositories[typeof(T)] = new FakeRepository<T>(entities);
        return this;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (!_repositories.TryGetValue(typeof(TEntity), out var repository))
        {
            repository = new FakeRepository<TEntity>();
            _repositories[typeof(TEntity)] = repository;
        }

        return (IGenericRepository<TEntity>)repository;
    }

    public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SaveCount++;
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => Task.CompletedTask;
    public Task<bool> CommitAsync() => Task.FromResult(true);
    public Task RollbackAsync() => Task.CompletedTask;
    public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters) => Task.FromResult(0);
    public Task<List<TResult>> ExecuteSqlInterpolatedAsync<TResult>(FormattableString sql) where TResult : class =>
        Task.FromResult(new List<TResult>());
    public void Dispose() { }
}

internal sealed class FakeRepository<T>(IEnumerable<T>? seed = null) : IGenericRepository<T> where T : class
{
    private readonly List<T> _items = seed?.ToList() ?? [];

    private IQueryable<T> Apply(ISpecification<T>? spec)
    {
        IQueryable<T> query = _items.AsQueryable();
        if (spec?.Criteria is not null) query = query.Where(spec.Criteria);
        if (spec?.OrderBy is not null) query = query.OrderBy(spec.OrderBy);
        if (spec?.OrderByDescending is not null) query = query.OrderByDescending(spec.OrderByDescending);
        if (spec?.IsPaginationEnabled == true) query = query.Skip(spec.Skip).Take(spec.Take);
        return query;
    }

    private static void Cancel(CancellationToken cancellationToken) => cancellationToken.ThrowIfCancellationRequested();

    public Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult<IReadOnlyList<T>>(Apply(spec).ToList());
    }

    public Task<T> GetByIdSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(Apply(spec).FirstOrDefault()!);
    }

    public Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector,
        ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(Apply(spec).Select(selector).ToList());
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult<IReadOnlyList<T>>(_items.ToList());
    }

    public IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec) => Apply(spec);

    public Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        var idProperty = typeof(T).GetProperty("Id");
        return Task.FromResult(_items.FirstOrDefault(item => Equals(idProperty?.GetValue(item), id))!);
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        _items.Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        _items.AddRange(entities);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities) => Task.CompletedTask;
    public Task UpdateAsync(T entity) => Task.CompletedTask;

    public Task DeleteAsync(T entity)
    {
        _items.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> CountWithSpec(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(Apply(spec).Count());
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(_items.Count);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(_items.AsQueryable().Any(predicate));
    }

    public Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(Apply(spec).Any());
    }

    public Task<TResult?> GetSingleProjectedAsync<TResult>(Expression<Func<T, TResult>> selector,
        ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
    {
        Cancel(cancellationToken);
        return Task.FromResult(Apply(spec).Select(selector).FirstOrDefault());
    }

    public Task<List<T>> FromSqlAsync(string sql, params object[] parameters) => Task.FromResult(new List<T>());
}
