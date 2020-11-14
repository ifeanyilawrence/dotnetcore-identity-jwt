using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.Repositories.Implementation
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        public RepositoryBase(ApplicationDbContext applicationDbContext)
        {
            this.ApplicationDbContext = applicationDbContext;
        }

        public IQueryable<T> FindAll()
        {
            return this.ApplicationDbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.ApplicationDbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            this.ApplicationDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.ApplicationDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.ApplicationDbContext.Set<T>().Remove(entity);
        }
    }
    public class Repository<TEntity> : IRepositoryAsync<TEntity>, IRepository<TEntity> where TEntity: class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            if (_context != null)
            {
                _dbSet = _context.Set<TEntity>();
            }
        }

        public virtual void Clear()
        {
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate) => Queryable().Count(predicate);

        public void Delete(object id, bool saveNow = true)
        {
            object[] objArray1 = new object[] { id };
            TEntity entity = _dbSet.Find(objArray1);
            Delete(entity, true);
            if (saveNow)
            {
                _context.SaveChanges();
            }
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            if (saveNow)
            {
                _context.SaveChanges();
            }
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool save = true)
        {
            _dbSet.RemoveRange(entities);
            if (save)
                SaveChanges();
        }

        public virtual IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? page = new int?(), int? pageSize = new int?())
        {
            IQueryable<TEntity> arg = _dbSet;
            if (orderBy != null)
            {
                arg = orderBy(arg);
            }
            if (predicate != null)
            {
                arg = Queryable().Where(predicate);
            }
            if ((page != null) && (pageSize != null))
            {
                arg = Queryable().Take(Queryable().Skip((page.Value - 1) * pageSize.Value).Count());
            }
            return arg;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate) => Queryable().SingleOrDefault(predicate);

        public virtual TEntity Find(params object[] keyValues) =>
            this._dbSet.Find(keyValues);

        public virtual TEntity Find(object id)
        {
            object[] objArray1 = new object[] { id };
            return this._dbSet.Find(objArray1);
        }

        public virtual Task<TEntity> FindAsync(params object[] keyValues) => FindAsync(CancellationToken.None, keyValues);

        public virtual Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues) => _dbSet.FindAsync(cancellationToken, keyValues);

        public virtual void InserRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            foreach (TEntity local in entities)
            {
                Insert(local, true);
            }
            if (saveNow)
                _context.SaveChanges();
        }

        public virtual void Insert(TEntity entity, bool saveNow = true)
        {
            _context.Entry(entity).State = EntityState.Added;
            if (saveNow)
                _context.SaveChanges();
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities, bool save = true)
        {
            _dbSet.AddRange(entities);
            if (save)
                SaveChanges();
        }

        public virtual IQueryable<TEntity> Queryable() => _dbSet;

        public void Reload(TEntity entity)
        {
            DbContext context = _context as DbContext;
            if (context != null)
            {
                context.Entry(entity).Reload();
            }
        }

        public virtual int SaveChanges() => _context.SaveChanges();

        public virtual void SaveChangesAsny()
        {
            _context.SaveChangesAsync();
        }

        public virtual Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            _context.Entry(entity).State = EntityState.Modified;
            if (saveNow) 
                _context.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool save = true)
        {
            if (save)
                SaveChanges();
        }

        public virtual IQueryable<TEntity> Table => Queryable();
    }
}
