using System;
using System.Collections.Generic;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public abstract class PercentageStateEvaluator : IStateEvaluator<double>
    {
        public abstract double Evaluate(double currentState, DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}
