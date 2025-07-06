using Shared.Domain.Ports;
using Metrics.Application.DTO;

namespace FileLogger.Infrastructure;

public class FileLoggerPlugin : IMonitorPlugin
{
    private readonly string _logFilePath;

    public FileLoggerPlugin()
    {
        _logFilePath = Path.Combine(AppContext.BaseDirectory, "metrics_log.txt");
    }

    public string Name => "FileLogger";

    public async Task OnMetricsCollectedAsync(SystemMetricsDTO metrics)
    {
        var line = $"[{metrics.Timestamp:HH:mm:ss}] CPU: {metrics.Cpu:F2}% | RAM: {metrics.UsedRam}/{metrics.TotalRam} MB | DISK: {metrics.UsedDisk}/{metrics.TotalDisk} MB";
        await File.AppendAllTextAsync(_logFilePath, line + Environment.NewLine);
    }
}
