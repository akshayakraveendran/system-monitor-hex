namespace Metrics.Domain.Models;

public class SystemMetrics
{
    public float CpuUsagePercent { get; set; }
    public float RamUsedMB { get; set; }
    public float RamTotalMB { get; set; }
    public float DiskUsedMB { get; set; }
    public float DiskTotalMB { get; set; }
    public DateTime Timestamp { get; set; }
}
