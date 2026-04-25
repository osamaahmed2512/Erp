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

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return;

            await _dbContext.Set<T>().AddRangeAsync(entities);
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

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).AsTracking().FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, bool asNoTracking = true)
        {
            var query = ApplySpecification(spec);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<int> CountWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }
        public IQueryable<T> GetQueryableWithSpec(ISpecification<T> spec)
        {
            return ApplySpecification(spec);
        }

        public async Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, TResult>> selector, ISpecification<T>? spec = null)
        {
            return await ApplySpecification(spec)
                         .AsNoTracking()
                         .Select(selector)
                         .ToListAsync();
        }

        public async Task<TResult?> GetSingleProjectedAsync<TResult>(
                                Expression<Func<T, TResult>> selector,
                                ISpecification<T>? spec = null)
        {
            return await ApplySpecification(spec)
                .AsNoTracking()
                .Select(selector)
                .FirstOrDefaultAsync();
        }


        private IQueryable<T> ApplySpecification(ISpecification<T>? spec)
        {
            return spec == null
                ? _dbContext.Set<T>().AsQueryable()
                : SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .AnyAsync(predicate);
        }

        public async Task<bool> AnyAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec)
                .AsNoTracking()
                .AnyAsync();
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
