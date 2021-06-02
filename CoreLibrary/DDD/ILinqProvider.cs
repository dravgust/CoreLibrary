using System;
using System.Linq;
using CoreLibrary.DDD.Entities;
using JetBrains.Annotations;

namespace CoreLibrary.DDD
{
    [PublicAPI]
    public interface ILinqProvider

    {
        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class, IHasId;


        IQueryable GetQueryable(Type t);
    }
}
