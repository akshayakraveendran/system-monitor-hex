using Metrics.Application.DTO;

namespace Shared.Domain.Ports;

public interface IMonitorPlugin
{
    string Name { get; }
    Task OnMetricsCollectedAsync(SystemMetricsDTO metrics);
}
