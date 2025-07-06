namespace Metrics.Application.DTO;

public class SystemMetricsDTO
{
    public float Cpu { get; set; }
    public float UsedRam { get; set; }
    public float TotalRam { get; set; }
    public float UsedDisk { get; set; }
    public float TotalDisk { get; set; }
    public DateTime Timestamp { get; set; }
}
