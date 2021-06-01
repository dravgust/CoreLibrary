using CoreLibrary.DDD.Entities;

namespace CoreLibrary.DDD.Specifications
{
    public class IdSpecification<TKey,T> : ExpressionSpecification<T>
        where T : IHasId<TKey>
    {
        public TKey Id { get; private set; }

        public IdSpecification(TKey id)
            : base(x => x.Id.Equals(id))
        {
            Id = id;
        }
    }
}
