using System.Collections.Generic;
using CoreLibrary.Utilities;

namespace CoreLibrary.Threading.Channels
{
    public class ChannelTelemetryBuilder<T> where T : ChannelTelemetrySnapshot
    {
        private readonly List<T> _snapshots;
        private readonly int _queueLength;

        public ChannelTelemetryBuilder(List<T> snapshots, int queueLength)
        {
            Guard.NotNull(snapshots, nameof(snapshots));
            _snapshots = snapshots;
            _queueLength = queueLength;
        }

        public ChannelTelemetrySnapshot Build() // 1-channel snapshots
        {
            var result = new ChannelTelemetrySnapshot();

            foreach (var snapshot in _snapshots) // consumer
            {
                if (snapshot.MaxTimeMs >= result.MaxTimeMs)
                {
                    result.MaxTimeMs = snapshot.MaxTimeMs;
                    if (result.MinTimeMs == 0)
                        result.MinTimeMs = snapshot.MaxTimeMs;
                }
                if (snapshot.MinTimeMs < result.MinTimeMs)
                    result.MinTimeMs = snapshot.MinTimeMs;

                result.OperationCount += snapshot.OperationCount;
                result.TotalPendingTimeMs += snapshot.TotalPendingTimeMs;
                result.DroppedItems += snapshot.DroppedItems;
            }
            result.ConsumersCount = _snapshots.Count;
            result.Length = _queueLength;

            if (result.OperationCount > 0)
                result.AverageTimePerOperationMs = result.TotalPendingTimeMs / result.OperationCount;

            return result;
        }
    }
}
