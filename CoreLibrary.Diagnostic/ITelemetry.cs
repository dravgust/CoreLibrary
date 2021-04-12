namespace CoreLibrary.Diagnostic
{
    public interface ITelemetry
    {
        void StartMeasurement();
        void StopMeasurement();

        ITelemetrySnapshot GetSnapshot();
    }
}
