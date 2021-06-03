using CoreLibrary.AutoMapper;
using CoreLibrary.Components.CQRS;
using CoreLibrary.DDD.Entities;

namespace CorLibrary.Test.Stubs
{
    public class Category : HasIdBase<int>
    {
        public Category()
        {
            
        }

        public Category(int rating, string name)
        {
            Rating = rating;
            Name = name;
        }

        public int Rating { get; set; }

        public string Name { get; set; }
    }

    public class Product : HasIdBase<int>
    {
        public Product()
        {
            
        }

        public Product(Category category, string name, decimal price)
        {
            Category = category;
            Name = name;
            Price = price;
        }

        public Category Category { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    [DtoFor(typeof(Product)), ConventionalMap(typeof(Product))]
    public class ProductDto : HasIdBase<int>
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int CategoryRating { get; set; }

        public decimal Price { get; set; }
    }

}
