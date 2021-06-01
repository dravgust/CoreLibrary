﻿using System;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.DDD
{
    public interface IUnitOfWork : IDisposable
    {
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IHasId;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class, IHasId;

        TEntity Find<TEntity>(object id)
            where TEntity : class, IHasId;

        IHasId Find(Type entityType, object id);

        void Commit();
    }
}