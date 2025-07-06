using Metrics.Domain.Models;

namespace Metrics.Domain.Ports;

public interface ISystemMetricsReader
{
    Task<SystemMetrics> GetCurrentMetricsAsync();
}
