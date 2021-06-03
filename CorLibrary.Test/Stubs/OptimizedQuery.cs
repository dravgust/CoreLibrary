using System.Threading.Tasks;
using CoreLibrary.CQRS;
using CoreLibrary.DDD.Pagination;

namespace CorLibrary.Test.Stubs
{
    //public class OptimizedQuery : IAsyncQuery<UberProductSpec, IPagedEnumerable<ProductDto>>
    //{
    //    public int Price { get; set; } = 5;


    //    public SqlConnection OpenConnection(int i)
    //    {
    //        var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    //        sqlConnection.Open();
    //        return sqlConnection;
    //    }

    //    public async Task<IPagedEnumerable<ProductDto>> Ask(UberProductSpec spec)
    //    {
    //        using var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    //        await sqlConnection.OpenAsync();
    //        var res1 = await sqlConnection
    //            .QueryAsync<ProductDto>("SELECT p.Id as Id, c.Name as CategoryName, p.Price as Price " +
    //                                    "FROM Products p INNER JOIN Categories c ON p.Category_Id = c.Id " +
    //                                    "WHERE Price > @Price " +
    //                                    "ORDER BY Id OFFSET 0 ROWS FETCH NEXT 30 ROWS ONLY", new { Price });

    //        var res2 = await sqlConnection.QueryAsync<int>("SELECT COUNT(*) FROM Products " +
    //                                                       "WHERE Price > @Price", new { Price });

    //        return Paged.From(res1, res2.Single());
    //    }
    //}
}
