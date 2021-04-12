using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CoreLibrary.Diagnostic.Monitoring
{
    public class StateEventArgs<TState> : EventArgs
    {
        private IReadOnlyCollection<string> FailedSensors { get; }
        public DateTime TimeStamp { get; }
        public TState State { get; }
        public StateEventArgs(TState state, DateTime timeStamp, IEnumerable<string> failedSensors)
        {
            State = state;
            TimeStamp = timeStamp;
            FailedSensors = failedSensors.ToImmutableList();
        }
    }
}
