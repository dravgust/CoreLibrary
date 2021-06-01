using System.Linq;

namespace CoreLibrary.Common
{
    public interface IProjector
    {
        IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable);
    }
}
