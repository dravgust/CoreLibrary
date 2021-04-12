using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;
using Polly;

namespace CorLibrary.Test
{
    public class ThreadTest
    {
        private volatile int l;
        long acc = 0;

        [Test]
        public void Test1()
        {
           
           
            //SpinWait.SpinUntil(() => 1 == 1, 1000);

            //*******************LOCK ****************
            long l = 0;

            //CAS - Compare-and-swap
            while (Interlocked.CompareExchange(ref l, 1, 0) == 0);
            acc++;
            //TAS - Test-and-Set
            Interlocked.Exchange(ref l, 0);

            //--------------OR

            //...
            while (Interlocked.CompareExchange(ref l, 1, 0) == 0);

            try
            {
                ; // doWork();
            }
            finally
            {
                l = 0;
            }
            //*******************LOCK-FREE****************
            long old;
            do
            {
                old = acc;
            } while (Interlocked.CompareExchange(ref acc, old + 1, old) == 0);
            //--------------OR
            var sw = new SpinWait();
            do
            {
                old = acc;
                sw.SpinOnce();//multi threads
            } while (Interlocked.CompareExchange(ref acc, old + 1, old) == 0);
        }

        [Test]
        public void RxTest()
        {
            Console.WriteLine("***");

            IObservable<DateTimeOffset> timestamps =
                Observable.Interval(TimeSpan.FromSeconds(1))
                    .Timestamp()
                    .Where(x => x.Value % 2 == 0)
                    .Select(x => x.Timestamp);

            using var t = timestamps.Subscribe(x => Console.WriteLine(x), ex => Console.WriteLine(ex));

            Thread.Sleep(10000);
            Console.WriteLine("done...");
        }

        [Test]
        public void DataflowTest()
        {
            try
            {
                var multiplyBlock = new TransformBlock<int, int>(item =>
                {
                    if (item == 1)
                        throw new InvalidOperationException("Blech.");
                    Console.WriteLine("add");
                    return item * 2;
                });
                
                var subtractBlock = new TransformBlock<int, int>(item =>
                {
                    Console.WriteLine("sub");
                    return item - 2;
                });
                multiplyBlock.LinkTo(subtractBlock, new DataflowLinkOptions() {PropagateCompletion = true});

                multiplyBlock.Post(2);
                
                Console.WriteLine("post");
                subtractBlock.Completion.Wait(10000);
                Console.WriteLine("done");
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Flatten().Message);
            }
        }

        public void RetryTest()
        {
            Policy.Handle<Exception>()
                .Retry(10, (e, i) => Console.WriteLine($"Error '{e.Message}' at retry #{i}"))
                .Execute(async () =>
                {
                    var client = new HttpClient();
                    return await client.GetStringAsync("url");
                });
        }

        //https://www.telerik.com/blogs/using-polly-for-net-resilience-and-transient-fault-handling-with-net-core
        public async Task<string> DownloadStringWithRetries(HttpClient client, string url)
        {
            var nextDelay = TimeSpan.FromSeconds(1);
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(nextDelay);
                nextDelay += nextDelay;
            }

            return await client.GetStringAsync(url);
        }

        public async Task<string> DownloadStringWithTimeout(HttpClient client, string uri)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Task<string> downloadTask = client.GetStringAsync(uri, cts.Token);
            Task timeoutTask = Task.Delay(Timeout.Infinite, cts.Token);

            Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);
            if (completedTask == timeoutTask)
                return null;
            return await downloadTask;
        }

    }

    



}