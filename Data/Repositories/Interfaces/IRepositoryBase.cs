using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table { get; }

        void Clear();
        void Delete(object id, bool save = true);
        void Delete(TEntity entity, bool save = true);
        void DeleteRange(IEnumerable<TEntity> entities, bool save = true);
        TEntity Find(params object[] keyValues);
        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity, bool save = true);
        void InsertRange(IEnumerable<TEntity> entities, bool save = true);
        IQueryable<TEntity> Queryable();
        void Reload(TEntity entity);
        int SaveChanges();
        void Update(TEntity entity, bool save = true);
        void UpdateRange(IEnumerable<TEntity> entities, bool save = true);
    }
    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? page = null, int? pageSize = null);
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
