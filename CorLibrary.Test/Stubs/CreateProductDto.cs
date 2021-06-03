using CoreLibrary.AutoMapper;

namespace CorLibrary.Test.Stubs
{
    [ConventionalMap(typeof(Product))]
    public class CreateProductDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}