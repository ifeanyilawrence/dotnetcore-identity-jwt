using Data.Repositories.Interfaces;
using LinqKit;
using System;
using System.Linq.Expressions;

namespace Data.Repositories.Implementation
{
    public abstract class QueryObject<TEntity> : IQueryObject<TEntity>
    {
        private Expression<Func<TEntity, bool>> _query;

        protected QueryObject()
        {
        }

        public IQueryObject<TEntity> And(IQueryObject<TEntity> queryObject) => And(queryObject.Expression);

        public IQueryObject<TEntity> And(Expression<Func<TEntity, bool>> query)
        {
            _query = (_query == null) ? query : PredicateBuilder.And<TEntity>(_query, Extensions.Expand<Func<TEntity, bool>>(query));
            return this;
        }

        public IQueryObject<TEntity> Or(IQueryObject<TEntity> queryObject) => this.Or(queryObject.Expression);

        public IQueryObject<TEntity> Or(Expression<Func<TEntity, bool>> query)
        {
            _query = (_query == null) ? query : PredicateBuilder.Or<TEntity>(_query, Extensions.Expand<Func<TEntity, bool>>(query));
            return this;
        }

        public virtual Expression<Func<TEntity, bool>> Expression => _query;
    }
}
