﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Metrics.Application.Services;
using Metrics.Domain.Ports;
using Metrics.Infrastructure.Readers;
using FileLogger.Infrastructure;
using ApiSender.Infrastructure;
using Shared.Domain.Ports;





var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ISystemMetricsReader, WindowsMetricsReader>();
        services.AddSingleton<MonitorService>();
        services.AddSingleton<IMonitorPlugin, FileLoggerPlugin>();
        services.AddHttpClient<IMonitorPlugin, ApiSenderPlugin>();        
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

var host = builder.Build();

var monitor = host.Services.GetRequiredService<MonitorService>();
var cancellationToken = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Shutting down...");
    cancellationToken.Cancel();
    e.Cancel = true;
};

while (!cancellationToken.IsCancellationRequested)
{
    var dto = await monitor.GetMetricsAsync();
    Console.WriteLine($"[{dto.Timestamp:T}] CPU: {dto.Cpu:F2}% | RAM: {dto.UsedRam}/{dto.TotalRam} MB | Disk: {dto.UsedDisk}/{dto.TotalDisk} MB");
    var plugins = host.Services.GetServices<IMonitorPlugin>();
    foreach (var plugin in plugins)
    {
        await plugin.OnMetricsCollectedAsync(dto);
    }
    Console.WriteLine("-------------------");

    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken.Token);
}

