using Metrics.Domain.Models;
using Metrics.Domain.Ports;
using System.Diagnostics;

namespace Metrics.Infrastructure.Readers;

public class LinuxMetricsReader : ISystemMetricsReader
{
    public async Task<SystemMetrics> GetCurrentMetricsAsync()
    {
        var cpu = await GetCpuUsageAsync();
        var (totalRam, usedRam) = GetRamInfo();

        var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.Name == "/").FirstOrDefault();
        float usedDisk = drives != null ? (drives.TotalSize - drives.TotalFreeSpace) / (1024f * 1024) : 0;
        float totalDisk = drives != null ? drives.TotalSize / (1024f * 1024) : 0;

        return new SystemMetrics
        {
            CpuUsagePercent = cpu,
            RamUsedMB = usedRam,
            RamTotalMB = totalRam,
            DiskUsedMB = usedDisk,
            DiskTotalMB = totalDisk,
            Timestamp = DateTime.UtcNow
        };
    }

    private async Task<float> GetCpuUsageAsync()
    {
        var cpu1 = ReadCpuStat();
        await Task.Delay(100);
        var cpu2 = ReadCpuStat();

        float idleDiff = cpu2.idle - cpu1.idle;
        float totalDiff = cpu2.total - cpu1.total;

        return (1.0f - (idleDiff / totalDiff)) * 100;
    }

    private (ulong idle, ulong total) ReadCpuStat()
    {
        var parts = File.ReadLines("/proc/stat")
            .First(line => line.StartsWith("cpu "))
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(ulong.Parse)
            .ToArray();

        ulong idle = parts[3]; // idle
        ulong total = parts.Aggregate((a, b) => a + b);
        return (idle, total);
    }

    private (float totalMB, float usedMB) GetRamInfo()
    {
        var meminfo = File.ReadAllLines("/proc/meminfo")
            .Select(line => line.Split(':'))
            .ToDictionary(
                parts => parts[0].Trim(),
                parts => float.Parse(parts[1].Trim().Split(' ')[0]) // in KB
            );

        float total = meminfo["MemTotal"] / 1024f;
        float free = (meminfo["MemFree"] + meminfo["Buffers"] + meminfo["Cached"]) / 1024f;
        float used = total - free;

        return (total, used);
    }
}
