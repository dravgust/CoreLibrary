using System.Linq;
using CoreLibrary.CQRS;
using CoreLibrary.DDD.Pagination;
using CoreLibrary.DDD.Specifications;

namespace CorLibrary.Test.Stubs
{
    public interface IController
    {

    }

    public class ProductController : IController
    {
        private readonly IQuery<int, ProductDto> _byId;
        private readonly IQuery<ProductSpec, IPagedEnumerable<ProductDto>> _bySpec;

        public ProductController(IQuery<int, ProductDto> byId
            , IQuery<ProductSpec, IPagedEnumerable<ProductDto>> bySpec)
        {
            _byId = byId;
            _bySpec = bySpec;
        }

        public dynamic ById(int id) => _byId.Ask(id);

        public dynamic BySpec(ProductSpec spec) =>_bySpec.Ask(spec);
    }

    public class ProductSpec
        : IdPaging<ProductDto>
        , ILinqSpecification<Product>
    {
        public IQueryable<ProductDto> Apply(IQueryable<ProductDto> query) => query
            .Where(x => x.Id > 1);

        public IQueryable<Product> Apply(IQueryable<Product> query) => query
            .Where(x => x.Category.Rating > 0);
    }
}