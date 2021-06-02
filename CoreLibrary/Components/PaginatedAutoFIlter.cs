using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary.CQRS;
using CoreLibrary.DDD.Entities;
using CoreLibrary.DDD.Specifications;
using JetBrains.Annotations;

namespace CoreLibrary.Components
{
    public class PaginatedAutoFilter<TKey, TDto>
        : IdPaging<TKey> 
        , ILinqSpecification<TDto> 
        where TKey : class, IHasId<int>
        where TDto : class
    {
        public IDictionary<string, object> Filter { get; }

        public PaginatedAutoFilter()
        {
            Filter = new Dictionary<string, object>();
        }

        public PaginatedAutoFilter(int page, int take, [NotNull] IDictionary<string, object> filter)
            :base(page,take)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }
        public IQueryable<TDto> Apply(IQueryable<TDto> query)
            => query.ApplyDictionary(Filter);
    }
}
