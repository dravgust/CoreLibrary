using System;
using System.Threading.Tasks;

namespace CoreLibrary.Diagnostic.Monitoring.Sensors
{
    public class Sensor : ISensor
    {
        private readonly Func<Task<SensorResult>> _func;
        public string Name { get; set; }

        public Sensor(string name, Func<Task<SensorResult>> func)
        {
            Name = name;
            _func = func;
        }

        public async Task<SensorResult> Check()
        {
            try
            {
                return await _func().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new SensorResult
                {
                    SensorName = Name,
                    Time = DateTime.UtcNow,
                    Success = false,
                    Data = string.Empty,
                    Exception = e
                };
            }
        }
    }
}
