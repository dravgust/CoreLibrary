using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoreLibrary.Diagnostic.Monitoring
{
    public class JournalRecord
    {
        public DateTime Time { get; set; }
        public IReadOnlyCollection<SensorResult> Values { get; set; }
        public override string ToString() => $"{Time}: [{string.Join(",", Values)}]";

        public JournalRecord(DateTime time, IEnumerable<SensorResult> values)
        {
            Time = time;
            Values = values.ToImmutableList();
        }
    }
}
