using System.Diagnostics;
using Metrics.Domain.Models;
using Metrics.Domain.Ports;
using Microsoft.VisualBasic.Devices;

namespace Metrics.Infrastructure.Readers;

public class WindowsMetricsReader : ISystemMetricsReader
{
    private readonly PerformanceCounter _cpuCounter = new("Processor", "% Processor Time", "_Total");
    private readonly PerformanceCounter _ramCounter = new("Memory", "Available MBytes");

    public async Task<SystemMetrics> GetCurrentMetricsAsync()
    {
        float totalRam = GetTotalRamMB();
        float availableRam = _ramCounter.NextValue();
        float usedRam = totalRam - availableRam;

        await Task.Delay(100); 
        float cpu = _cpuCounter.NextValue();

        
        return new SystemMetrics
        {
            CpuUsagePercent = cpu,
            RamUsedMB = usedRam,
            RamTotalMB = totalRam,
            DiskUsedMB = 50000f,
            DiskTotalMB = 100000f,
            Timestamp = DateTime.UtcNow
        };
    }

    private float GetTotalRamMB() =>
        new ComputerInfo().TotalPhysicalMemory / (1024f * 1024f);
}
