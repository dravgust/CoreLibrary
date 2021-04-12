using System;
using System.Threading;
using CoreLibrary.Diagnostic;
using CoreLibrary.Threading.Channels.Handlers;
using CoreLibrary.Utilities;

namespace CoreLibrary.Threading.Channels
{
    public class TelemetryChannel<T> : ProducerConsumerChannel<T>
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        public TelemetryChannel(string channelName,
            Action<T, CancellationToken> consumeAction,
            uint workerThreads = 1,
            bool enableTaskManagement = false) : base(channelName, workerThreads, enableTaskManagement)
        {
            _consumeAction = Guard.NotNull(consumeAction, nameof(consumeAction));
        }

        protected override void OnDataReceived(T item, CancellationToken token)
        {
            try
            {
                _consumeAction.Invoke(item, token);
            }
            catch (OperationCanceledException) { }
        }

        public bool Queue(T item)
        {
            return EnQueue(item);
        }
    }

    public class TelemetryChannel<T, TH> : ProducerConsumerChannel<T> where TH : ChannelHandler<T>, new()
    {
        private readonly TH _handler = new TH();
        private readonly HandlerTelemetry _handlerTelemetry;

        public TelemetryChannel(string channelName,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false) : base(channelName,
            startedNumberOfWorkerThreads, enableTaskManagement)
        {
            _handlerTelemetry = new HandlerTelemetry();
        }

        protected override void OnDataReceived(T item, CancellationToken token)
        {
            try
            {
                _handlerTelemetry.StartMeasurement();

                _handler.HandleAction(item, token);
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                _handlerTelemetry.StopMeasurement();
            }
        }

        public bool Queue(T item)
        {
            return EnQueue(item);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _handler.Dispose();
        }

        public bool ShouldBeCleared => _handler.CanBeCleared;

        public override ITelemetrySnapshot GetSnapshot()
        {
            var queueSnapshot = (ChannelTelemetrySnapshot)base.GetSnapshot();
            var snapshot = new ChannelHandlerTelemetrySnapshot
            {
                HandlerTelemetrySnapshot = (HandlerTelemetrySnapshot)_handlerTelemetry.GetSnapshot(),
                MinTimeMs = queueSnapshot.MinTimeMs,
                MaxTimeMs = queueSnapshot.MaxTimeMs,
                Length = queueSnapshot.Length,
                TotalPendingTimeMs = queueSnapshot.TotalPendingTimeMs,
                OperationCount = queueSnapshot.OperationCount,
                AverageTimePerOperationMs = queueSnapshot.AverageTimePerOperationMs,
                ConsumersCount = queueSnapshot.ConsumersCount,
                DroppedItems = queueSnapshot.DroppedItems
            };

            return snapshot;
        }
    }
}
