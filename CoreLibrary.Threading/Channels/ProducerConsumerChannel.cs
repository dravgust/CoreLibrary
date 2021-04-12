using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CoreLibrary.Diagnostic;
using CoreLibrary.Threading.Channels.Consumers;
using CoreLibrary.Utilities;

namespace CoreLibrary.Threading.Channels
{
    public abstract class ProducerConsumerChannel<T>
    {
        private const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty;

        private readonly ConcurrentBag<TelemetryConsumer<T>> _workers = new();

        private readonly int _maxWorkers = 10;
        private readonly int _maxQueue = 50000;
        private readonly int _consumerManagementTimeoutMs =  10000;

        private readonly Channel<TelemetryItem<T>> _channel;

        private readonly PropertyInfo _itemsCountForDebuggerOfReader;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly System.Timers.Timer _timer;
        private readonly string _channelName;
        private int _droppedItems;

        protected ProducerConsumerChannel(string channelName, uint workerThreads = 1, bool enableTaskManagement = false)
        {
            Guard.Assert(workerThreads > 0, $"{nameof(workerThreads)} must be > 0");

            _channelName = channelName ?? GetType().Name;

            var options = new BoundedChannelOptions(_maxQueue)
            {
                SingleWriter = true,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.DropOldest
            };
            _channel = Channel.CreateBounded<TelemetryItem<T>>(options);
            _itemsCountForDebuggerOfReader = _channel.Reader.GetType().GetProperty("ItemsCountForDebugger", BindFlags);
            _cancellationSource = new CancellationTokenSource();

            for (var i = 0; i < workerThreads; i++)
            {
                var w = new TelemetryConsumer<T>(_channel, ConsumerName, OnDataReceived, _cancellationSource.Token);

                _workers.Add(w);
                w.StartMeasurement();
                w.StartConsume();
            }

            Trace.TraceInformation($"[{_channelName}] started with {workerThreads} consumers. Options: maxWorkers: {_maxWorkers}, maxQueueLength: {_maxQueue}, consumerManagementTimeout: {_consumerManagementTimeoutMs} ms");

            _timer = new System.Timers.Timer { Interval = _consumerManagementTimeoutMs };
            _timer.Elapsed += (sender, e) => ManageWorkers();

            if (enableTaskManagement)
                _timer.Start();
        }

        protected abstract void OnDataReceived(T item, CancellationToken token);

        protected bool EnQueue(T item)
        {
            var t = new TelemetryItem<T>(item) { StartTime = DateTime.Now };
            if (_channel.Writer.TryWrite(t))
                return true;

            _droppedItems++;
            return false;
        }

        private string ConsumerName => $"{_channelName}Consumer: {Create.ShortUID()}";

        private void ManageWorkers()
        {
            _timer.Stop();

            try
            {
                var count = Count;

                var requiredWorkers = count / 2; // 1thread = 2 handles/sec
                var workersDiff = _workers.Count - requiredWorkers;

                if (Math.Abs(workersDiff) < 10)
                {
                    // Debug.WriteLine($"[ProducerConsumersChannel] no action for diff. of {workersDiff} workers, now workers:{_workers.Count}, queue:{count}");
                    return;
                }

                var processedWorkers = 0;
                if (workersDiff > 0) // there are more then required
                {
                    for (var i = 0; i < workersDiff; i++)
                    {
                        if (_workers.Count > 1 && _workers.TryTake(out var worker))
                        {
                            worker.StopRequest(false);
                            processedWorkers++;
                        }

                    }
                    
                    Debug.WriteLine($"[{_channelName}] Removed {processedWorkers} workers, now workers:{_workers.Count} because {count} queue");
                }
                else if (workersDiff < 0) // missing workers, need to add
                {
                    if (_workers.Count >= _maxWorkers)
                        return;

                    workersDiff = -workersDiff;
                    for (var i = 0; i < workersDiff; i++)
                    {
                        if (_workers.Count >= _maxWorkers)
                            break;

                        var w = new TelemetryConsumer<T>(_channel.Reader, ConsumerName, OnDataReceived, _cancellationSource.Token);
                        _workers.Add(w);
                        w.StartConsume();
                        processedWorkers++;
                    }
                    Debug.WriteLine($"[{_channelName}] Added {processedWorkers} workers, now workers:{_workers.Count} because {count} queue");
                }
            }
            finally
            {
                _timer.Start();
            }
        }

        public int Count => (int)_itemsCountForDebuggerOfReader.GetValue(_channel.Reader);

        public virtual void Shutdown()
        {
            try
            {
                _timer.Stop();
                _channel.Writer.Complete();
                _cancellationSource.Cancel();
                StopMeasurement();
                var consumerTasks = new List<Task>();
                while (!_workers.IsEmpty)
                {
                    _workers.TryTake(out var w);
                    if (w != null)
                        consumerTasks.Add(w.GetTask());
                }

                Task.WaitAll(consumerTasks.ToArray());
            }
            catch (Exception e)
            {
                Trace.TraceInformation($"[{_channelName}.Shutdown]: {e.Message}");
            }
        }

        public virtual void StopMeasurement()
        {
            foreach (var telemetryConsumer in _workers)
                telemetryConsumer.StopMeasurement();
        }

        public virtual ITelemetrySnapshot GetSnapshot()
        {
            var snapshots = _workers.Select(w => w.GetSnapshot()).Cast<ChannelTelemetrySnapshot>().ToList();
            var result = new ChannelTelemetryBuilder<ChannelTelemetrySnapshot>(snapshots, Count).Build();
            result.DroppedItems = _droppedItems;
            _droppedItems = 0;

            return result;
        }
    }
}
