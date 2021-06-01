﻿using System;
using System.Linq.Expressions;

namespace CoreLibrary.DDD
{
    public enum SortOrder
    {
        Asc = 1,
        Desc = 2
    }

    public class Sorting<TEntity, TKey>
        where TEntity : class
    {
        public Expression<Func<TEntity, TKey>> Expression { get; private set; }

        public SortOrder SortOrder { get; private set; }

        public Sorting(
            Expression<Func<TEntity, TKey>> expression,
            SortOrder sortOrder = SortOrder.Asc)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            SortOrder = sortOrder;
        }
    }
}
