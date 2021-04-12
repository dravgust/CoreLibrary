using System;

namespace CoreLibrary.Diagnostic
{
    public class TelemetryItem<T>
    {
        public TelemetryItem(T data)
        {
            Data = data;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int HandleDuration => (int)(EndTime - StartTime).TotalMilliseconds;

        public T Data { get; }
    }
}
