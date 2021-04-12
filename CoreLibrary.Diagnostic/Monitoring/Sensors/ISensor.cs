using System.Threading.Tasks;

namespace CoreLibrary.Diagnostic.Monitoring.Sensors
{
    public interface ISensor
    {
        string Name { get; }

        Task<SensorResult> Check();
    }
}
