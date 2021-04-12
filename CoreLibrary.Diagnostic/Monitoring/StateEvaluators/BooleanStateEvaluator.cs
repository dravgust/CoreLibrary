using System;
using System.Collections.Generic;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public abstract class BooleanStateEvaluator : IStateEvaluator<bool>
    {
        public abstract bool Evaluate(bool currentState, DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}
