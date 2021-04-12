namespace CoreLibrary.Diagnostic
{
    public interface ITelemetrySnapshot
    {
        int Length { get; }
        long MaxTimeMs { get; }
        long MinTimeMs { get; }
        long OperationCount { get; }
    }
}
