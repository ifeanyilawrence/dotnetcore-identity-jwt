using System;
using System.Linq.Expressions;

namespace Data.Repositories.Interfaces
{
    public interface IQueryObject<TEntity>
    {
        IQueryObject<TEntity> And(IQueryObject<TEntity> queryObject);
        IQueryObject<TEntity> And(Expression<Func<TEntity, bool>> query);
        IQueryObject<TEntity> Or(IQueryObject<TEntity> queryObject);
        IQueryObject<TEntity> Or(Expression<Func<TEntity, bool>> query);

        Expression<Func<TEntity, bool>> Expression { get; }
    }
}
