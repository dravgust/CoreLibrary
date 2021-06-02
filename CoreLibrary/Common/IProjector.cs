using System.Linq;
using JetBrains.Annotations;

namespace CoreLibrary.Common
{
    [PublicAPI]
    public interface IProjector
    {
        IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable);
    }
}
