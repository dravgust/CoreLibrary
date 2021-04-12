using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public class ServiceStateEvaluator : IStateEvaluator<ServiceState>
    {
        public ServiceState Evaluate(ServiceState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
        {
            //ar data = journal.TakeLast(3).ToImmutableList();
            var data = journal.OrderByDescending(i=>i.Time).Take(3).ToImmutableList();

            var totalChecks = data.Count;
            var failedChecks = data.Count(o => o.Values.Any(v => v.Success == false));

            if (failedChecks == 0)
                return ServiceState.Live;

            if (failedChecks == 1)
                return ServiceState.Warning;

            return ServiceState.Down;
        }
    }
}
