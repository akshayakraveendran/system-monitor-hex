using Metrics.Application.DTO;
using Metrics.Domain.Ports;

namespace Metrics.Application.Services;

public class MonitorService
{
    private readonly ISystemMetricsReader _reader;

    public MonitorService(ISystemMetricsReader reader)
    {
        _reader = reader;
    }

    public async Task<SystemMetricsDTO> GetMetricsAsync()
    {
        var metrics = await _reader.GetCurrentMetricsAsync();

        return new SystemMetricsDTO
        {
            Cpu = metrics.CpuUsagePercent,
            UsedRam = metrics.RamUsedMB,
            TotalRam = metrics.RamTotalMB,
            UsedDisk = metrics.DiskUsedMB,
            TotalDisk = metrics.DiskTotalMB,
            Timestamp = metrics.Timestamp
        };
    }
}
