using CoreLibrary.Diagnostic;
using CoreLibrary.Threading.Channels.Handlers;

namespace CoreLibrary.Threading.Channels
{
    public class ChannelTelemetrySnapshot : ITelemetrySnapshot
    {
        public int ConsumersCount { set; get; }
        public long AverageTimePerOperationMs { set; get; }
        public long DroppedItems { set; get; }

        public int Length { get; set; }
        public long TotalPendingTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public long MinTimeMs { get; set; }
        public long OperationCount { get; set; }
    }

    public class ChannelHandlerTelemetrySnapshot : ChannelTelemetrySnapshot
    {
        public HandlerTelemetrySnapshot HandlerTelemetrySnapshot { get; set; }
    }
}
