using System;

namespace CoreLibrary.Diagnostic.Monitoring
{
    public class SensorResult
    {
        public string SensorName { get; set; }
        public DateTime Time { get; set; }
        public bool Success { get; set; }
        public string Data { get; set; }
        public Exception Exception { get; set; }

        public override string ToString() => $"{Time}: {Success} => {Data}";
    }
}
