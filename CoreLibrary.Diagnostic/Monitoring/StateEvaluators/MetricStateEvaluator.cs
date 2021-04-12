#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public class MetricStateEvaluator : IStateEvaluator<MetricState>
    {
        public MetricState Evaluate(MetricState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
        {
            var last = journal.Cast<JournalRecord?>().LastOrDefault();

            if (last == null)
                return MetricState.Normal;

            return last.Values.Any(o => !o.Success)
                ? MetricState.Danger
                : MetricState.Normal;
        }
    }
}
