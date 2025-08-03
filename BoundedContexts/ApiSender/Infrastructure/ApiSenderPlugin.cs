using Shared.Domain.Ports;
using Metrics.Application.DTO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;


namespace ApiSender.Infrastructure;

public class ApiSenderPlugin : IMonitorPlugin
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;

    public ApiSenderPlugin(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _endpointUrl = config.GetValue<string>("ApiSender:Endpoint") ?? throw new ArgumentNullException("Endpoint URL is missing");

        if (string.IsNullOrEmpty(_endpointUrl))
            throw new ArgumentNullException(nameof(_endpointUrl), "Endpoint URL is missing");

        Console.WriteLine($"[ApiSender] Using endpoint: {_endpointUrl}");
    }


    public string Name => "ApiSender";

    public async Task OnMetricsCollectedAsync(SystemMetricsDTO metrics)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(metrics),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var response = await _httpClient.PostAsync(_endpointUrl, content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiSender] Endpoint error: {ex.Message}");
        }
    }
}
