namespace CoreLibrary.DDD.Entities
{
    public abstract class HasIdBase<T> : IHasId<T>
    {
        public T Id { get; set; }

        public bool IsNew()
        {
            return Id == null || Id.Equals(default(T));
        }

        object IHasId.Id => Id;
    }
}
