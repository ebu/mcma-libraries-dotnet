namespace Mcma.Storage;

public readonly struct StreamProgress
{
    public StreamProgress(long current, long total)
    {
        Current = current;
        Total = total > 0 ? total : long.MaxValue;
    }
        
    public long Current { get; }
        
    public long Total { get; }

    public bool IsIndeterminate => Total == long.MaxValue;

    public double PercentCompleted => (double)Current / Total;
}