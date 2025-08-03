# System Monitor – Plugin-Based Console Application

A cross-platform system monitor built with .NET, following clean and modular architecture. The app monitors system CPU, RAM, and disk usage in real time and uses a plugin system to send this data to various endpoints (file, HTTP API, etc.).

---

## Features

- Real-time monitoring of:
  - CPU usage (%)
  - RAM usage (used / total in MB)
  - Disk usage (used / total in MB)
- Console output every few seconds (configurable)
- Plugin architecture using `IMonitorPlugin`
- Sample Plugins:
  - `FileLoggerPlugin` – logs to a local file
  - `ApiSenderPlugin` – posts to an external REST API (configurable)
- Configuration via `appsettings.json`
- Clean Hexagonal Architecture with separate bounded contexts

---

## Architecture Overview

We used **Hexagonal Architecture (a.k.a. Ports & Adapters)**, with strong boundaries between:

- **Domain**: Core models and interfaces (e.g., `SystemMetrics`, `ISystemMetricsReader`)
- **Application**: Services that coordinate logic (`MonitorService`)
- **Infrastructure**: Platform-specific code and plugin implementations
- **Shared**: Cross-cutting abstractions like `IMonitorPlugin`

### Bounded Contexts

- `Metrics`: System monitoring logic and readers
- `FileLogger`: Plugin that logs to a file
- `ApiSender`: Plugin that sends data to API
- `Shared`: Contains the shared plugin interface
- `ConsoleApp`: Console UI and orchestration

All layers communicate using DTOs (e.g., `SystemMetricsDTO`) to enforce separation of concerns.

---

## How to Build & Run

### Prerequisites

- [.NET SDK 9.0 or later](https://dotnet.microsoft.com)
- Windows (for full metrics support via `PerformanceCounter`)

### Steps

```bash
git clone https://github.com/your-username/SystemMonitor.git
cd SystemMonitor

# Restore and build
dotnet restore
dotnet build

# Run the app
dotnet run --project ConsoleApp
----

# Create an API endpoint to listen to updates
NOTE: I have used webhook.site for this
- Go to https://webhook.site
- Copy the temporary url
- paste it in ConsoleApp/appsettings.json for ApiSender.Endpoint


Configuration
Edit ConsoleApp/appsettings.json:

{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "System.Net.Http.HttpClient": "None"
    }
  },
  "MonitorIntervalSeconds": 5,
  "ApiSender": {
    "Endpoint": "https://webhook.site/58460f43-3e81-4fb3-9488-3f16d6ef6d9d"
  },
  "FileLogger": {
    "LogFilePath": "metrics_log.txt"
  }
}

# Output
- Logged to console
- Logged to metrics_log.txt
- Sent as JSON to API (if configured)