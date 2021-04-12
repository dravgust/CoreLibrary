using System;
using System.Collections.Generic;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public interface IStateEvaluator<TState>
    {
        TState Evaluate(TState currentState, DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}
