using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreLibrary.Diagnostic.Monitoring.Sensors;
using CoreLibrary.Diagnostic.Monitoring.StateEvaluators;

namespace CoreLibrary.Diagnostic.Monitoring.Monitors
{
    public class MonitorBase<TState> : IMonitor<TState> where TState : struct, IConvertible
    {
        private TState _state;
        private readonly IList<ISensor> _sensors;
        private readonly IStateEvaluator<TState> _stateEvaluator;
        private readonly List<JournalRecord> _journal;
        private readonly ReaderWriterLockSlim _journalLock;
        private readonly ReaderWriterLockSlim _stateLock;
        private readonly Stopwatch _stopwatch;

        public event EventHandler<StateEventArgs<TState>> StateChanged;
        public event EventHandler<HealthCheckEventArgs> HealthChecked;

        public MonitorBase(IStateEvaluator<TState> stateEvaluator, TimeSpan retentionPeriod, TState initialState)
        {
            RetentionPeriod = retentionPeriod;
            _state = initialState;
            StateChangedDate = DateTime.UtcNow;

            _stateEvaluator = stateEvaluator;
            _stopwatch = Stopwatch.StartNew();
            _sensors = new List<ISensor>();
            _journal = new List<JournalRecord>();
            _journalLock = new ReaderWriterLockSlim();
            _stateLock = new ReaderWriterLockSlim();
        }

        public TState State
        {
            get
            {
                _stateLock.EnterReadLock();

                try
                {
                    return _state;
                }
                finally
                {
                    _stateLock.ExitReadLock();
                }
            }
        }

        public TimeSpan UpTime => _stopwatch.Elapsed;
        public string Name { get; set; }
        public void AddSensor(ISensor sensor) => _sensors.Add(sensor);

        protected virtual void ChangeState(TState state, IEnumerable<string> failedSensors)
        {
            _stateLock.EnterWriteLock();

            try
            {
                _state = state;
            }
            finally
            {
                _stateLock.ExitWriteLock();
            }

            StateChangedDate = DateTime.UtcNow;

            StateChanged?.Invoke(this, new StateEventArgs<TState>(state, StateChangedDate, failedSensors));
        }

        public void CheckHealth()
        {
            var results = new Stack<SensorResult>();

            var tasks = _sensors
                .Select(async o => { results.Push(await o.Check().ConfigureAwait(false)); })
                .ToArray();

            Task.WaitAll(tasks);

            var now = DateTime.UtcNow;

            _journalLock.EnterWriteLock();

            try
            {
                _journal.RemoveAll(o => o.Time < now.Subtract(RetentionPeriod));

                _journal.Add(new JournalRecord(now, results));
            }
            finally
            {
                _journalLock.ExitWriteLock();
            }

            var state = _stateEvaluator.Evaluate(State, StateChangedDate, _journal);

            if (!EqualityComparer<TState>.Default.Equals(State, state))
            {
                ChangeState(state, results.Where(o => !o.Success).Select(o => o.SensorName));
            }

            OnHealthChecked(now, results);
        }

        protected virtual void OnHealthChecked(DateTime now, IReadOnlyCollection<SensorResult> results) =>
            HealthChecked?.Invoke(this, new HealthCheckEventArgs(now, results));

        public IReadOnlyCollection<JournalRecord> Journal
        {
            get
            {
                _journalLock.EnterReadLock();
                try
                {
                    return _journal;
                }
                finally
                {
                    _journalLock.ExitReadLock();
                }
            }
        }

        public DateTime StateChangedDate { get; private set; }
        public TimeSpan RetentionPeriod { get; private set; }
    }
}
