using CoreLibrary.DDD.Entities;
using CoreLibrary.DDD.Pagination;

namespace CoreLibrary.DDD.Specifications
{
    public class IdPaging<TEntity>: IdPaging<TEntity, int>
        where TEntity : class, IHasId<int>
    {
        public IdPaging(int page, int take)
            : base(page, take)
        {
        }
        
        public IdPaging()
        {
        }
    }

    public class IdPaging<TEntity, TKey>: Paging<TEntity, TKey> 
        where TEntity : class, IHasId<TKey>
    {
        public IdPaging(int page, int take)
            : base(page, take, new Sorting<TEntity, TKey>(x => x.Id, SortOrder.Desc))
        {
        }

        public IdPaging()
        {
        }

        protected override Sorting<TEntity, TKey> BuildDefaultSorting()
        {
            return new Sorting<TEntity, TKey>(x => x.Id, SortOrder.Desc);
        }
    }
}
