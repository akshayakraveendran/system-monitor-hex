using Metrics.Application.DTO;

namespace FileLogger.Domain.Ports;

public interface IMonitorPlugin
{
    string Name { get; }
    Task OnMetricsCollectedAsync(SystemMetricsDTO metrics);
}
