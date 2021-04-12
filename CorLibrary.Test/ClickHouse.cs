using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Dapper;
using System.Collections;
using System.Diagnostics;
using System.Text;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;


namespace CorLibrary.Test
{
    public class ClickHouse : AbstractConnectionTestFixture
    {
        public static IEnumerable<TestCaseData> SimpleSelectQueries => TestUtilities.GetDataTypeSamples()
            .Where(s => ShouldBeSupportedByDapper(s.ClickHouseType))
            .Where(s => s.ExampleValue != DBNull.Value)
            .Where(s => !s.ClickHouseType.StartsWith("Array")) // Dapper issue, see ShouldExecuteSelectWithParameters test
            .Select(sample => new TestCaseData($"SELECT {{value:{sample.ClickHouseType}}}", sample.ExampleValue));

        // "The member value of type <xxxxxxxx> cannot be used as a parameter value"
        private static bool ShouldBeSupportedByDapper(string clickHouseType)
        {
            if (clickHouseType.StartsWith("Tuple"))
                return false;
            switch (clickHouseType)
            {
                case "UUID":
                case "Date":
                case "Nothing":
                case "IPv4":
                case "IPv6":
                    return false;
                default:
                    return true;
            }
        }

        public async Task<(long records, long time)> StartTest()
        {
            var records = 100;
            var connection2 = TestUtilities.GetTestClickHouseConnection();
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            for (var i = 1; i <= records; i++)
            {
                await connection2.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO logger.log VALUES ({log_time:DateTime}, {app_id:Int32}, {log_level:String}, {logger_name:String}, {message:String})";
                command.AddParameter("log_time", DateTime.Now);
                command.AddParameter("app_id", "0");
                command.AddParameter("log_level", "info");
                command.AddParameter("logger_name", "default");
                command.AddParameter("message", $"test message {i}");
                await command.ExecuteNonQueryAsync();

                await connection2.CloseAsync();
            }
            connection2.Dispose();

            stopWatch.Stop();

            Console.WriteLine($"Elapsed {records} records in {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)}");

            return (records, stopWatch.ElapsedMilliseconds);
        }

        public async Task<long> StartTestBatch()
        {
            var records = 1000000;
            var stopWatch = new Stopwatch();
            try
            {

                var connection2 = TestUtilities.GetTestClickHouseConnection();
                var insertStr = new StringBuilder("INSERT INTO logger.log VALUES");
                stopWatch.Start();
                for (var i = 1; i <= records; i++)
                {
                    var dt = DateTime.Now;
                    insertStr.Append($"('2021-03-12 11:{dt.Minute:00}:{dt.Second:00}', 0, 'info', 'default', 'test message {i}')"); 
                }

                await connection2.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = insertStr.ToString();
                await command.ExecuteNonQueryAsync();

                await connection2.CloseAsync();
                connection2.Dispose();

                Console.WriteLine($"Elapsed {records} records in {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)}");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                stopWatch.Stop();
            }
            return stopWatch.ElapsedMilliseconds;
        }

        [Test]
        public void ShouldExecuteSimpleInsert()
        {
            var task = StartTestBatch().Result;

            Console.WriteLine($"Done! => {task}");

            //using var reader = await connection.ExecuteReaderAsync("SELECT * FROM test.nested ORDER BY id ASC");
            //Assert.IsTrue(reader.Read());
            //var values = reader.GetFieldValues();
            //Assert.AreEqual(1, values[0]);
            //CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, values[1] as IEnumerable);
            //CollectionAssert.AreEquivalent(new[] { "v1", "v2", "v3" }, values[2] as IEnumerable);
        }

        [Test]
        public async Task ShouldExecuteSimpleSelect()
        {
            string sql = "SELECT * FROM system.table_functions";

            var functions = (await connection.QueryAsync<string>(sql)).ToList();
            CollectionAssert.IsNotEmpty(functions);
            CollectionAssert.AllItemsAreNotNull(functions);
        }

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(ClickHouse), nameof(SimpleSelectQueries))]
        public async Task ShouldExecuteSelectWithSingleParameterValue(string sql, object value)
        {
            var parameters = new Dictionary<string, object> { { "value", value } };
            var results = await connection.QueryAsync<string>(sql, parameters);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0}", value), results.Single());
        }

        [Test]
        public async Task ShouldExecuteSelectWithArrayParameter()
        {
            var parameters = new Dictionary<string, object> { { "names", new[] { "mysql", "odbc" } } };
            string sql = "SELECT * FROM system.table_functions WHERE has({names:Array(String)}, name)";

            var functions = (await connection.QueryAsync<string>(sql, parameters)).ToList();
            CollectionAssert.IsNotEmpty(functions);
            CollectionAssert.AllItemsAreNotNull(functions);
        }

        [Test]
        [Ignore("Requires Dapper support, see https://github.com/StackExchange/Dapper/pull/1467")]
        public async Task ShouldExecuteSelectReturningArray()
        {
            string sql = "SELECT array(1,2,3)";
            var result = (await connection.QueryAsync<int[]>(sql)).Single();
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AllItemsAreNotNull(result);
        }
    }
}