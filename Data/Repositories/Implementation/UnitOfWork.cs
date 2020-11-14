using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class UnitOfWork<TContext> : IUnitOfWorkAsync<TContext>, IUnitOfWork<TContext>, IDisposable where TContext : DbContext
    {
        private TContext _dataContext;
        private bool _disposed;
        private DbConnection _dbConnection;
        private DbTransaction _transaction;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            _dataContext = context;
            _repositories = new Dictionary<Type, object>();
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _dbConnection = _dataContext.Database.GetDbConnection();
            if(_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            _transaction = _dbConnection.BeginTransaction(isolationLevel);
        }

        public bool Commit()
        {
            _dataContext.SaveChanges();
            _transaction.Commit();
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
                        {
                            _dbConnection.Close();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    if (_dataContext != null)
                    {
                        _dataContext.Dispose();
                        _dataContext = null;
                    }
                }
                _disposed = true;
            }
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(_dataContext);
            return (IRepository<TEntity>)_repositories[type];
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(_dataContext);
            return (IRepositoryAsync<TEntity>)_repositories[type];
        }

        public void Rollback()
        {
            try
            {
                _dataContext.SaveChanges();
                _transaction.Rollback();
            }
            catch (Exception)
            {
            }
        }
        public TContext Context { get; }

        public int SaveChanges() => _dataContext.SaveChanges();

        public Task<int> SaveChangesAsync() => _dataContext.SaveChangesAsync();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _dataContext.SaveChangesAsync(cancellationToken);
    }
}
