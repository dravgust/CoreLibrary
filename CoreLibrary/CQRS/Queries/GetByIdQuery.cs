using System;
using System.Linq;
using CoreLibrary.Common;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.CQRS.Queries
{
    public class GetByIdQuery<TKey, TEntity, TResult> : IQuery<TKey, TResult>
        where TKey : struct, IComparable, IComparable<TKey>, IEquatable<TKey>
        where TEntity : class, IHasId<TKey>
        where TResult : IHasId<TKey>
    {
        protected readonly ILinqProvider LinqProvider;

        protected readonly IProjector Projector;

        public GetByIdQuery(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider ?? throw new ArgumentNullException(nameof(linqProvider));
            Projector = projector ?? throw new ArgumentNullException(nameof(projector));
        }

        public virtual TResult Ask(TKey specification) =>
            Projector.Project<TEntity, TResult>(LinqProvider
                .GetQueryable<TEntity>()
                .Where(x => specification.Equals(x.Id)))
            .SingleOrDefault();
    }
}
