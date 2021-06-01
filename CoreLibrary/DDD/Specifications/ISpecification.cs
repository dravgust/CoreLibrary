namespace CoreLibrary.DDD.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T o);
    }
}