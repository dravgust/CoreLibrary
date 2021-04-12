using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLibrary.Tools
{
    public class Supplier : IDisposable
    {
        private readonly int _minTimeOut;
        private readonly int _maxTimeOut;
        private CancellationTokenSource _cts;
        private readonly IQueue _queue;
        private string _id;

        public Supplier(string id, IQueue queue, int minTimeOut, int maxTimeOut)
        {
            _id = id ?? Guid.NewGuid().ToString();
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));

            if (minTimeOut < 0)
                throw new ArgumentException("invalid minTimeOut", nameof(minTimeOut));
            _minTimeOut = minTimeOut;

            if (maxTimeOut < 0)
                throw new ArgumentException("invalid minTimeOut", nameof(maxTimeOut));
            _maxTimeOut = maxTimeOut;
        }

        public void Start(CancellationToken token)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            
            Task.Factory.StartNew(async () =>
                    await Run(_cts.Token),
                _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        protected async Task Run(CancellationToken token)
        {
            try
            {
                var rnd = new Random();
 
                while (true)
                {
                    await Task.Delay(rnd.Next(_minTimeOut, _maxTimeOut), token);

                    _queue.Enqueue($"[{_id}] {DateTime.Now}");

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (TaskCanceledException e)
            {

            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            //GC.SuppressFinalize(this);
        }
    }
}
