using CoreLibrary.Diagnostic;

namespace CoreLibrary.Threading.Channels.Handlers
{
    public class HandlerTelemetrySnapshot : ITelemetrySnapshot
    {
        public int Length { get; set; }
        public long MeasurementTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public long MinTimeMs { get; set; }
        public long OperationCount { get; set; }
    }
}
