using CoreLibrary.DDD.Entities;
using JetBrains.Annotations;

namespace CoreLibrary.DDD.Specifications
{
    [PublicAPI]
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
