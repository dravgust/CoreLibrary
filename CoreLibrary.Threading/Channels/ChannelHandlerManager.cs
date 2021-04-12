using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoreLibrary.Threading.Channels.Handlers;

namespace CoreLibrary.Threading.Channels
{
    public class ChannelHandlerManager<T, TIdent, TH> where TH : ChannelHandler<T>, new()
    {
        private const int ChannelManagementIntervalMin = 35 * 60 * 1000;

        private readonly ConcurrentDictionary<TIdent, TelemetryChannel<T, TH>> _channels = new ConcurrentDictionary<TIdent, TelemetryChannel<T, TH>>();
        private Timer _timer;

        public ChannelHandlerManager()
        {
            _timer = new Timer(OnTimerCallback, null, ChannelManagementIntervalMin, ChannelManagementIntervalMin);
        }

        private void OnTimerCallback(object state)
        {
            ClearHandlers();
        }

        public Func<T, bool> this[TIdent key]
        {
            get
            {
                if (!_channels.TryGetValue(key, out var channel))
                {
                    channel = Factory(key);
                    _channels.TryAdd(key, channel);
                }

                return channel.Queue;
            }
        }

        public bool Queue(TIdent key, T item)
        {
            if (!_channels.TryGetValue(key, out var channel))
            {
                channel = Factory(key);
                _channels.TryAdd(key, channel);
            }

            return channel.Queue(item);
        }

        public QueueHandlerTelemetryReport GeTelemetryReport()
        {
            var snapshots = _channels.Values.Select(w => w.GetSnapshot())
                .Cast<ChannelHandlerTelemetrySnapshot>().ToList();

            return new ReportTelemetryBuilder<ChannelHandlerTelemetrySnapshot>(snapshots).Build();
        }

        protected TelemetryChannel<T, TH> Factory(TIdent key)
        {
            var type = typeof(TelemetryChannel<T, TH>);

            var constructor = type.GetConstructor(new[] { typeof(string), typeof(uint), typeof(bool) });
            if (constructor != null)
            {
                return (TelemetryChannel<T, TH>)Activator.CreateInstance(type, key, (uint)1, false);
            }

            return Activator.CreateInstance<TelemetryChannel<T, TH>>();
        }

        public bool ShouldBeCleared(TIdent key)
        {
            return _channels.TryGetValue(key, out var channel) && channel.ShouldBeCleared;
        }

        public void ClearChannel(TIdent key)
        {
            try
            {
                if (_channels.TryRemove(key, out var channel))
                {
                    channel.Shutdown();
                }
            }
            catch { }
        }

        private void ClearHandlers()
        {
            var counter = 0;
            foreach (var channel in _channels)
            {
                if (channel.Value.ShouldBeCleared)
                {
                    ClearChannel(channel.Key);
                    counter++;
                }
            }

            Debug.WriteIf(counter > 0, $"{typeof(TH).Name} | Cleared {counter} controllers{Environment.NewLine}");
        }

        public void Shutdown()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}
