using System;
using CoreLibrary.Diagnostic;

namespace CoreLibrary.Threading.Channels.Handlers
{
    public class HandlerTelemetry : ITelemetry
    {
        private readonly object _lock = new object();

        private DateTime _startDate = DateTime.MinValue;

        public long TimeCount { private set; get; }

        public long MaxTime { private set; get; }

        public long MinTime { private set; get; }

        public long OpCount { private set; get; }

        public void StartMeasurement()
        {
            _startDate = DateTime.Now;
        }

        public void StopMeasurement()
        {
            var timeSpan = (long)(DateTime.Now - _startDate).TotalMilliseconds;
            if (timeSpan >= MaxTime)
            {
                MaxTime = timeSpan;
            }

            if (MinTime == 0 || timeSpan < MinTime)
            {
                MinTime = timeSpan;
            }

            TimeCount += timeSpan;
            OpCount++;
        }

        public ITelemetrySnapshot GetSnapshot()
        {
            var result = new HandlerTelemetrySnapshot();

            lock (_lock)
            {
                result.MaxTimeMs = this.MaxTime;
                result.MinTimeMs = this.MinTime;
                result.MeasurementTimeMs = this.TimeCount;
                result.OperationCount = this.OpCount;

                Reset();
            }

            return result;
        }

        private void Reset()
        {
            MaxTime = 0;
            MinTime = 0;
            TimeCount = 0;
            OpCount = 0;
        }
    }
}
