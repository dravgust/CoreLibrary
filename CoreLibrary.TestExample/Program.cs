using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CoreLibrary.Diagnostic.Monitoring;
using CoreLibrary.Diagnostic.Monitoring.Monitors;
using CoreLibrary.Diagnostic.Monitoring.Sensors;
using CoreLibrary.Diagnostic.Monitoring.StateEvaluators;
using CoreLibrary.Threading.Channels;
using CoreLibrary.Threading.Channels.Consumers;
using HarabaSourceGenerators.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.TestExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddHostedService<SampleService>();
                    services.AddHostedService<SampleService2>();

                    services.AddSingleton(o =>
                    {
                        var channel = new TelemetryChannel<string>(null, async (data, token) =>  await Task.Delay(1000, token));
                        return channel;
                    });

                    //services.AddSingleton(new Sensor(nameof(Sensor), () =>
                    //{
                    //    var result = new SensorResult
                    //    {
                    //        Time = DateTime.UtcNow,
                    //        SensorName = nameof(Sensor),
                    //        Success = false
                    //    };

                    //    try
                    //    {
                    //        result.Success = true;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        result.Exception = e;
                    //        result.Data = e.Message;
                    //    }

                    //    return Task.FromResult(result);
                    //}));

                    services.AddSingleton(o =>
                    {
                        var channel = o.GetService<TelemetryChannel<string>>();
                        var sensor = new ChannelQueueCountSensor(channel);

                        return sensor;
                    });

                    services.AddSingleton<IStateEvaluator<bool>>(new TelemetryStateEvaluator());
                    services.AddSingleton<IObservable<EventPattern<HealthCheckEventArgs>>>(o =>
                    {
                        var stateEvaluator = o.GetService<IStateEvaluator<bool>>();
                        var defaultSensor = o.GetService<ChannelQueueCountSensor>();
                        var checkPeriod = TimeSpan.FromSeconds(10);

                        var observer = new TelemetryMonitor(checkPeriod, stateEvaluator, false);

                        var telemetryMonitor =
                            Observable.FromEventPattern<HealthCheckEventArgs>(
                                handler => observer.HealthChecked += handler,
                                handler => observer.HealthChecked -= handler);

                        //var telemetryMonitor =
                        //    Observable.FromEventPattern(observer, nameof(TelemetryMonitor.HealthChecked));

                        observer.AddSensor(defaultSensor);

                        observer.Start();
                        return telemetryMonitor;
                    });

                    
                    services.AddSingleton(new ServiceSensor());
                    services.AddSingleton<IStateEvaluator<MetricState>>(new MetricStateEvaluator());

                    services.AddSingleton(o =>
                    {
                        var stateEvaluator = o.GetService<IStateEvaluator<MetricState>>();
                        var serviceSensor = o.GetService<ServiceSensor>();
                      
                        var checkHealthPeriod = TimeSpan.FromMilliseconds(1000);
                        var retentionPeriod = TimeSpan.FromMinutes(5);

                        var observer = new ServiceObserver(checkHealthPeriod, stateEvaluator, retentionPeriod, MetricState.Normal);
                        observer.AddSensor(serviceSensor);
                        

                        //observer.Start();

                        return observer;
                    });
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();



            await host.RunAsync();
        }

        public class ChannelQueueCountSensor : Sensor
        {
            public ChannelQueueCountSensor(TelemetryChannel<string> channel) : base(nameof(ChannelQueueCountSensor), () =>
            {
                var result = new SensorResult
                {
                    Time = DateTime.UtcNow,
                    SensorName = nameof(ChannelQueueCountSensor),
                    Success = true
                };

                try
                {
                    var snapshot = (ChannelTelemetrySnapshot)channel.GetSnapshot();
                    result.Data = JsonSerializer.Serialize(snapshot);
                }
                catch (Exception e)
                {
                    result.Exception = e;
                    result.Success = false;
                }

                return Task.FromResult(result);
            }) { }
        }

        public class SampleService2 : IHostedService
        {
            private IDisposable _telemetry;

            //[Inject]
            private readonly IObservable<EventPattern<HealthCheckEventArgs>> _telemetryMonitor;


            public SampleService2(IObservable<EventPattern<HealthCheckEventArgs>> telemetryMonitor)
            {
                _telemetryMonitor = telemetryMonitor;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _telemetry = _telemetryMonitor.Subscribe(data =>
                        Console.WriteLine($"OnNext Checked: [{string.Join(", ", ((HealthCheckEventArgs)data.EventArgs).Results)}]"),
                    ex => Console.WriteLine("On Error: " + ex.Message),
                    () => Console.WriteLine("OnCompleted"));

                Observable.Interval(TimeSpan.FromSeconds(1))
                    .Buffer(2)
                    .Subscribe(x => Console.WriteLine(
                        $"{DateTime.Now.Second}: Got {x[0]} and {x[1]}"));

                Console.WriteLine("service started.");
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _telemetry?.Dispose();

                Console.WriteLine("service stopped.");
                return Task.CompletedTask;
            }
        }

        public class SampleService : IHostedService
        {
            private readonly System.Timers.Timer _timer;
            private readonly ServiceObserver _observer;
            private readonly TelemetryChannel<string> _channel;


            public SampleService(ServiceObserver observer, TelemetryChannel<string> channel)
            {
                _observer = observer;
                _channel = channel;
                _timer = new System.Timers.Timer(1000);

                //_observer.StateChanged += (sender, e) =>
                //{
                //    Console.ForegroundColor = e.State == MetricState.Danger ? ConsoleColor.Red : ConsoleColor.Green;
                //    Console.WriteLine($"MetricState changed: {e.State}");
                //    Console.ForegroundColor = ConsoleColor.White;
                //};

                //_observer.HealthChecked += (sender, e) =>
                //{
                //    Console.WriteLine($"Health checked: [{string.Join(", ", e.Results)}]");
                //};

                //Console.Clear();
                //Console.ForegroundColor = ConsoleColor.Gray;
            }
            public Task StartAsync(CancellationToken cancellationToken)
            {
                _timer.Elapsed += (sender, e) =>
                {
                    //JsonSerializer.Serialize(_observer.Journal)
                    _channel.Queue($"{_observer.Name} UpTime: {_observer.UpTime} State: {_observer.State} Journal: {_observer.Journal.Count}");
                };
                _timer.Start();

                Console.WriteLine("service started.");

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _timer.Stop();
                _timer.Dispose();

                Console.WriteLine("service stopped.");
                return Task.CompletedTask;
            }
        }

        public class ServiceObserver : TimerMonitor<MetricState>
        {
            public ServiceObserver(TimeSpan checkHealthPeriod, IStateEvaluator<MetricState> stateEvaluator, TimeSpan retentionPeriod, MetricState initialState)
                : base(checkHealthPeriod, stateEvaluator, retentionPeriod, initialState)
            {
                Name = nameof(ServiceObserver);
            }
        }

        public class TelemetryMonitor : TimerMonitor<bool>
        {
            public TelemetryMonitor(TimeSpan checkHealthPeriod, IStateEvaluator<bool> stateEvaluator, bool initialState)
                : base(checkHealthPeriod, stateEvaluator, checkHealthPeriod, initialState)
            {
                Name = this.GetType().Name;
            }
        }

        public class TelemetryStateEvaluator : IStateEvaluator<bool>
        {
            public bool Evaluate(bool currentState, DateTime stateChangedLastTime,
                IReadOnlyCollection<JournalRecord> journal) => true;
        }

        public class ServiceSensor : ISensor
        {
            //private readonly ProducerConsumerChannel<T> _channel;
            public string Name => $"{this.GetType().Name}";

            public ServiceSensor()
            {
                //_channel = channel;
            }

            public Task<SensorResult> Check()
            {
                var result = new SensorResult
                {
                    Time = DateTime.UtcNow,
                    SensorName = this.Name,
                    Success = false
                };

                try
                {
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Exception = e;
                    result.Data = e.Message;
                }

                return Task.FromResult(result);
            }
        }


    }
}
