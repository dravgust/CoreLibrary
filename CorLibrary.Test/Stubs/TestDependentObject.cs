using System.Collections.Generic;
using CoreLibrary.CQRS;
using CoreLibrary.DDD.Pagination;
using CoreLibrary.DDD.Specifications;

namespace CorLibrary.Test.Stubs
{
    public class TestDependentObject
    {
        public TestDependentObject(IQuery<IdPaging<ProductDto>, IPagedEnumerable<ProductDto>> pagedQuery
            , IQuery<object, IEnumerable<ProductDto>> projectionQuery
            , IQuery<int, ProductDto> getQuery
            , ICommandHandler<ProductDto, int> createOrUpdateCommandHandler)
        {
        }
    }
}
