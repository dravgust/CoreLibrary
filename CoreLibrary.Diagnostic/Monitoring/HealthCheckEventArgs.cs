using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoreLibrary.Diagnostic.Monitoring
{
    public class HealthCheckEventArgs
    {
        public DateTime TimeStamp { get; }

        public IReadOnlyCollection<SensorResult> Results { get; }

        public HealthCheckEventArgs(DateTime timeStamp, IReadOnlyCollection<SensorResult> results)
        {
            TimeStamp = timeStamp;
            Results = results ?? ImmutableList<SensorResult>.Empty;
        }
    }
}
