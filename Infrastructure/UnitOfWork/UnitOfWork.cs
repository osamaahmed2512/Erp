using Domain.Interfaces.Repositories;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private IDbContextTransaction _transaction;
        private readonly AppDbContext _dbContext;
        private Dictionary<string, object> repositories;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            repositories = new Dictionary<string, object>();
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
        }
        public async Task<bool> CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
                return true;
            }
            catch
            {
                await _transaction.RollbackAsync();
                return false;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }

        }
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _dbContext.Database.RollbackTransactionAsync();
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }
        public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var nameEntity = typeof(TEntity).Name;
            if (!repositories.ContainsKey(nameEntity))
            {
                var typeEntity = typeof(GenericRepository<>);
                var Instance = Activator.CreateInstance(typeEntity.MakeGenericType(typeof(TEntity)), _dbContext);
                repositories.Add(nameEntity, Instance!);
            }
            return (IGenericRepository<TEntity>)repositories[nameEntity];
        }
        public async Task<List<TResult>> ExecuteSqlInterpolatedAsync<TResult>(FormattableString sql) where TResult : class
        {
            return await _dbContext.Database
                .SqlQuery<TResult>(sql)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

    }
}
