using Shared.Domain.Ports;
using Metrics.Application.DTO;
using Microsoft.Extensions.Configuration;

namespace FileLogger.Infrastructure;

public class FileLoggerPlugin : IMonitorPlugin
{
    private readonly string _logFilePath;

    public FileLoggerPlugin(IConfiguration config)
    {
        var configPath = config.GetValue<string>("FileLogger:LogFilePath");
        if (string.IsNullOrWhiteSpace(configPath))
        {
            configPath = Path.Combine("", "metrics_log.txt");
        }

        _logFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", configPath));

        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public string Name => "FileLogger";

    public async Task OnMetricsCollectedAsync(SystemMetricsDTO metrics)
    {
        var line = $"[{metrics.Timestamp:HH:mm:ss}] CPU: {metrics.Cpu:F2}% | RAM: {metrics.UsedRam}/{metrics.TotalRam} MB | DISK: {metrics.UsedDisk}/{metrics.TotalDisk} MB";
        await File.AppendAllTextAsync(_logFilePath, line + Environment.NewLine);
        Console.WriteLine($"[FileLogger] Log path: {_logFilePath}");
    }
}
