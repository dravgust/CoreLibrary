using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace CoreLibrary.Tools
{
    public interface IQueue
    {
        void Enqueue(string message);
    }

    public delegate void NewDataProcessed(string value);
    public class SuppliersManager : IQueue, IDisposable
    {
        private readonly ConcurrentQueue<string> _queue = new();
        private readonly ManualResetEventSlim _event = new(false);
        private readonly int _threadCount;
        private readonly int _minTimeOut;
        private readonly int _maxTimeOut;
        private readonly Supplier[] _suppliers;
        public event NewDataProcessed NewDataProcessed;
        private CancellationTokenSource _cts;

        public SuppliersManager(int threadCount, int minTimeOut, int maxTimeOut)
        {
            if (threadCount < 0)
                throw new ArgumentException("invalid number of threads", nameof(threadCount));
            _threadCount = threadCount;

            if (minTimeOut < 0)
                throw new ArgumentException("invalid minTimeOut", nameof(minTimeOut));
            _minTimeOut = minTimeOut;

            if (maxTimeOut < 0)
                throw new ArgumentException("invalid minTimeOut", nameof(maxTimeOut));
            _maxTimeOut = maxTimeOut;

            _suppliers = new Supplier[threadCount];
            
        }
        
        public void Enqueue(string message)
        {
            _queue.Enqueue(message);
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Factory.StartNew(async () =>
                    await Run(_cts.Token),
                _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            for (var i = 0; i < _threadCount; i++)
            {
                var supplier = new Supplier($"{i}", this, _minTimeOut, _maxTimeOut);
                supplier.Start(token);

                _suppliers[i] = supplier;
            }
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
                    if (_queue.TryDequeue(out var message))
                    {
                        NewDataProcessed?.Invoke($"{message} | queue[{_queue.Count}]");

                        await Task.Delay(rnd.Next(_minTimeOut, _maxTimeOut), token);
                    }

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
            _event.Dispose();
        }
    }
}
