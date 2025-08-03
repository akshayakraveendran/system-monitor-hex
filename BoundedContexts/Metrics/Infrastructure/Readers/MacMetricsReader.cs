using Metrics.Domain.Models;
using Metrics.Domain.Ports;
using System.Diagnostics;

namespace Metrics.Infrastructure.Readers;

public class MacMetricsReader : ISystemMetricsReader
{
    public async Task<SystemMetrics> GetCurrentMetricsAsync()
    {
        float cpu = await GetCpuUsageAsync();
        var (totalRam, usedRam) = GetRamInfo();

        var (usedDisk, totalDisk) = GetDiskInfo();

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
        var startIdle = GetCpuTimes();
        await Task.Delay(100);
        var endIdle = GetCpuTimes();

        float idleDiff = endIdle.idle - startIdle.idle;
        float totalDiff = endIdle.total - startIdle.total;

        return (1.0f - (idleDiff / totalDiff)) * 100;
    }

    private (float idle, float total) GetCpuTimes()
    {
        var output = Execute("top -l 1 | grep 'CPU usage'");
        var parts = output.Split(','); // Ex: CPU usage: 10.68% user, 5.34% sys, 83.97% idle
        var idleStr = parts.FirstOrDefault(p => p.Contains("idle"));
        float idle = float.Parse(idleStr.Replace("idle", "").Replace("%", "").Trim());
        return (idle, 100f);
    }

    private (float total, float used) GetRamInfo()
    {
        string totalStr = Execute("sysctl -n hw.memsize").Trim();
        float totalBytes = float.Parse(totalStr);
        float totalMB = totalBytes / (1024 * 1024);

        var vmStats = Execute("vm_stat");
        var pageSize = 4096f; // default

        var dict = vmStats.Split('\n')
            .Where(line => line.Contains(":"))
            .ToDictionary(
                line => line.Split(':')[0].Trim(),
                line => float.Parse(line.Split(':')[1].Replace(".", "").Trim()) * pageSize / (1024 * 1024)
            );

        float free = dict.GetValueOrDefault("Pages free", 0);
        float inactive = dict.GetValueOrDefault("Pages inactive", 0);
        float used = totalMB - (free + inactive);

        return (totalMB, used);
    }

    private (float used, float total) GetDiskInfo()
    {
        var output = Execute("df -k /");
        var lines = output.Split('\n');
        if (lines.Length < 2) return (0, 0);

        var parts = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        float total = float.Parse(parts[1]) / 1024;
        float used = float.Parse(parts[2]) / 1024;

        return (used, total);
    }

    private string Execute(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}
