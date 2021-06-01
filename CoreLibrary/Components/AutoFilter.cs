using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using CoreLibrary.CQRS;

namespace CoreLibrary.Components
{
    public static class AutoFilterExtensions
    {
        public static IQueryable<T> ApplyDictionary<T>(this IQueryable<T> query
            , IDictionary<string, object> filters)
        {
            foreach (var kv in filters)
            {
                query = query.Where(kv.Value is string
                    ? $"{kv.Key}.StartsWith(@0)"
                    : $"{kv.Key}=@0", kv.Value);
            }
            return query;
        }

        public static IDictionary<string, object> GetFilters(this object o) => o.GetType()
            .GetTypeInfo()
            .GetProperties(BindingFlags.Public)
            .Where(x => x.CanRead)
            .ToDictionary(k => k.Name, v => v.GetValue(o));
    }

    public class AutoFilter<T> : ILinqSpecification<T>
        where T : class
    {
        public IDictionary<string, object> Filter { get; }

        public AutoFilter()
        {
            Filter = new Dictionary<string, object>();
        }

        public AutoFilter(IDictionary<string, object> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public IQueryable<T> Apply(IQueryable<T> query)
            => query.ApplyDictionary(Filter);
    }
}
