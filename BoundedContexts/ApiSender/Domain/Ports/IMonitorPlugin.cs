using Metrics.Application.DTO;

namespace ApiSender.Domain.Ports;

public interface IMonitorPlugin
{
    string Name { get; }
    Task OnMetricsCollectedAsync(SystemMetricsDTO metrics);
}
