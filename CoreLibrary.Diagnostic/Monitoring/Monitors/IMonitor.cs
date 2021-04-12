using System;
using CoreLibrary.Diagnostic.Monitoring.Sensors;

namespace CoreLibrary.Diagnostic.Monitoring.Monitors
{
    public interface IMonitor<TState> where TState: struct, IConvertible
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;
        event EventHandler<HealthCheckEventArgs> HealthChecked;

        TState State { get; }
        TimeSpan UpTime { get; }
        string Name { get; }

        void AddSensor(ISensor sensor);
        void CheckHealth();
    }
}
