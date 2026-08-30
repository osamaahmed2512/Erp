using Domain.Interfaces.Repositories;
using Domain.Interfaces.Specification;
using Infrastructure.Contexts;
using Infrastructure.Evaluator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
                return;

            await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return Task.CompletedTask;

            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }


        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().FindAsync([id], cancellationToken);
        }

        public async Task<T> GetByIdSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).AsTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(spec);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<int> CountWithSpec(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().CountAsync(cancellationToken);
        }
        public IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec)
        {
            return ApplySpecification(spec);
        }

        public async Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec)
                         .AsNoTracking()
                         .Select(selector)
                         .ToListAsync(cancellationToken);
        }

        public async Task<TResult?> GetSingleProjectedAsync<TResult>(
                                Expression<Func<T, TResult>> selector,
                                ISpecification<T>? spec = null,
                                CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec)
                .AsNoTracking()
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }


        private IQueryable<T> ApplySpecification(ISpecification<T>? spec)
        {
            return spec == null
                ? _dbContext.Set<T>().AsQueryable()
                : SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .AnyAsync(predicate, cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec)
                .AsNoTracking()
                .AnyAsync(cancellationToken);
        }
        public async Task<List<T>> FromSqlAsync(string sql, params object[] parameters)
        {

            return await _dbContext.Set<T>()
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
