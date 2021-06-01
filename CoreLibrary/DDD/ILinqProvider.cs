using System;
using System.Linq;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.DDD
{
    public interface ILinqProvider

    {
        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class, IHasId;


        IQueryable GetQueryable(Type t);
    }
}
