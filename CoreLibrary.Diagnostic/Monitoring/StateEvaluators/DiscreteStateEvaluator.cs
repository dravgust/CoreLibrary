using System;
using System.Collections.Generic;

namespace CoreLibrary.Diagnostic.Monitoring.StateEvaluators
{
    public abstract class DiscreteStateEvaluator : IStateEvaluator<bool>
    {
        protected readonly int Min;
        protected readonly int Max;
        protected readonly int Step;

        protected DiscreteStateEvaluator(int min, int max, int step)
        {
            Min = min;
            Max = max;
            Step = step;
        }

        public abstract bool Evaluate(bool currentState, DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}
