using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary.Common;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;
using CoreLibrary.DDD.Pagination;
using CoreLibrary.Extensions;

namespace CoreLibrary.CQRS.Queries
{
    public class ProjectionQuery<TSpecification, TSource, TDest>
        : IQuery<TSpecification, IEnumerable<TDest>>
        , IQuery<TSpecification, int>
        where TSource : class, IHasId
        where TDest : class
    {
        protected readonly ILinqProvider LinqProvider;
        protected readonly IProjector Projector;

        public ProjectionQuery(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider ?? throw new ArgumentNullException(nameof(linqProvider));
            Projector = projector ?? throw new ArgumentNullException(nameof(projector));
        }

        protected virtual IQueryable<TDest> GetQueryable(TSpecification spec)
        => LinqProvider
            .GetQueryable<TSource>()
            .ApplyIfPossible(spec)
            .Project<TSource, TDest>(Projector)
            .ApplyIfPossible(spec);

        public virtual IEnumerable<TDest> Ask(TSpecification specification)
            => GetQueryable(specification).ToArray();

        int IQuery<TSpecification, int>.Ask(TSpecification specification)
            => GetQueryable(specification).Count();
    }
}
