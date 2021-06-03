using System.Threading.Tasks;
using CoreLibrary.CQRS;

namespace CorLibrary.Test.Stubs
{
    //public class OptimizedCommnadHandler : IAsyncCommandHandler<CreateProductDto, int>
    //{

    //    public int Price { get; set; } = 5;


    //    public async Task<int> Handle(CreateProductDto input)
    //    {
    //        using var sqlConnection
    //            = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    //        await sqlConnection.OpenAsync();
    //        return await sqlConnection.ExecuteAsync("INSERT INTO Products(Category_Id, Name, Price) " +
    //                                                "VALUES (@CategoryId, @Name, @Price)", new
    //                                                {
    //                                                    CategoryId = input.CategoryId,
    //                                                    Name = input.Name,
    //                                                    Price = input.Price
    //                                                });
    //    }
    //}
}
