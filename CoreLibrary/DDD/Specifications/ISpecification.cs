using JetBrains.Annotations;

namespace CoreLibrary.DDD.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy([NotNull]T o);
    }
}