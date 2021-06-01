using System.Linq;

namespace CoreLibrary.CQRS
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
