using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

namespace Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Dispose(bool disposing);
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        void Rollback();
        int SaveChanges();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }

    public interface IUnitOfWorkAsync<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
    {
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
