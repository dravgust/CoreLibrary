using System;
using CoreLibrary.Diagnostic.Monitoring.StateEvaluators;

namespace CoreLibrary.Diagnostic.Monitoring.Monitors
{
    public interface IAutomatedTelemetryObserver<TState> : IMonitor<TState>
        where TState : struct, IConvertible
    {
        void Start();
    }

    public class TimerMonitor<TState> : MonitorBase<TState> , IAutomatedTelemetryObserver<TState> where TState : struct, IConvertible
    {
        public TimeSpan CheckHealthPeriod { get; }

        private readonly System.Timers.Timer _timer;

        public TimerMonitor(
            TimeSpan checkHealthPeriod,
            IStateEvaluator<TState> stateEvaluator,
            TimeSpan retentionPeriod, TState initialState) :
            base(stateEvaluator, retentionPeriod, initialState)
        {
            CheckHealthPeriod = checkHealthPeriod;

            _timer = new System.Timers.Timer(CheckHealthPeriod.TotalMilliseconds);
            _timer.Elapsed += (_, _) => CheckHealth();
            _timer.AutoReset = true;
        }

        public void Start() => _timer.Start();
    }
}
