using System;
using System.Reflection;
using System.Threading;
using gRPC;
using Grpc.Core;
using Grpc.Net.Client;
using NUnit.Framework;

namespace CorLibrary.Test
{
    public class GrpcClientTest
    {
        [Test]
        public void Test1()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var data0 = new HelloRequest { Name = "Anton" };
            var reply = client.SayHello(data0);

            Console.WriteLine("Ответ сервера: " + reply.Message);

            var c = new ServiceClient<Greeter.GreeterClient>("https://localhost:5001");
            var data = new HelloRequest {Name = "Anton2"};
            var result = c.Get(r => r.SayHello(data));

            Console.WriteLine("Ответ сервера2: " + result.Message);

            //var client = new Greeter.GreeterClient(channel);
            //using var call = client.SayHellos(new HelloRequest { Name = "World" });

            //while (await call.ResponseStream.MoveNext())
            //{
            //    Console.WriteLine("Greeting: " + call.ResponseStream.Current.Message);
            //    // "Greeting: Hello World" is written multiple times
            //}
        }
    }

    public class ServiceClient<TClient> where TClient : ClientBase
    {
        private readonly string _connectionString;

        public ServiceClient(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(connectionString);
            _connectionString = connectionString;
        }

        public TReturn Get<TReturn>(Func<TClient, TReturn> code)
        {

            using var channel = GrpcChannel.ForAddress(_connectionString);
            Type type = typeof(TClient);
            ConstructorInfo client = type.GetConstructor(new[] { typeof(GrpcChannel) });
            TClient instance = (TClient)client?.Invoke(new object[] { channel });
            var result = code(instance);
            return result;
        }
    }
}