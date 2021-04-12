using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using CoreLibrary.Diagnostic.Monitoring;
using CoreLibrary.Diagnostic.Monitoring.Monitors;
using CoreLibrary.Diagnostic.Monitoring.Sensors;
using CoreLibrary.Diagnostic.Monitoring.StateEvaluators;
using CoreLibrary.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace CorLibrary.Test
{
    public class DiagnosticTest
    {
        private volatile int l;
        long acc = 0;

        [Test]
        public async Task Test1()
        {
            var ctx = new CancellationTokenSource(120000);

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(o =>
                    {
                        var checkHealthPeriod = TimeSpan.FromMilliseconds(500);
                        var stateEvaluator = new MetricStateEvaluator();
                        var retentionPeriod = TimeSpan.FromMinutes(1);

                        var observer = new ServiceObserver(checkHealthPeriod, stateEvaluator, retentionPeriod, MetricState.Normal);


                        var serviceSensor = new ServiceSensor();
                        observer.AddSensor(serviceSensor);

                        observer.Start();

                        return observer;
                    });
                }).Build();

            

            await host.RunAsync(ctx.Token);

        }



        public class ServiceObserver : TimerMonitor<MetricState>
        {
            public ServiceObserver(TimeSpan checkHealthPeriod, IStateEvaluator<MetricState> stateEvaluator, TimeSpan retentionPeriod, MetricState initialMetricState)
                : base(checkHealthPeriod, stateEvaluator, retentionPeriod, initialMetricState)
            {
            }
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
                    Console.WriteLine($"{result.Time}");
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