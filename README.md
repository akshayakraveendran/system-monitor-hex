# System Monitor – Plugin-Based Console Application

A cross-platform system monitor built with .NET, following clean and modular architecture. The app monitors system CPU, RAM, and disk usage in real time and uses a plugin system to send this data to various endpoints (file, HTTP API, etc.).

---

## 🚀 Features

- ✅ Real-time monitoring of:
  - CPU usage (%)
  - RAM usage (used / total in MB)
  - Disk usage (used / total in MB)
- ✅ Console output every few seconds (configurable)
- ✅ Plugin architecture using `IMonitorPlugin`
- ✅ Sample Plugins:
  - `FileLoggerPlugin` – logs to a local file
  - `ApiSenderPlugin` – posts to an external REST API (configurable)
- ✅ Configuration via `appsettings.json`
- ✅ Clean Hexagonal Architecture with separate bounded contexts

---

## 🏗️ Architecture Overview

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

## 🧪 How to Build & Run

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

Configuration
Edit ConsoleApp/appsettings.json:

json
Copy
Edit
{
  "MonitorIntervalSeconds": 5,
  "ApiSender": {
    "Endpoint": "https://your-api-endpoint.com/receive-metrics"
  }
}
Output
Every 5 seconds:

Metrics are printed to console

Logged to metrics_log.txt

Sent as JSON to API (if configured)

Example payload:

json
Copy
Edit
{
  "cpu": 40.12,
  "usedRam": 5340.82,
  "totalRam": 6086.82,
  "usedDisk": 50000.0,
  "totalDisk": 100000.0,
  "timestamp": "2025-05-14T14:32:01Z"
}
🧠 Design Decisions
Chose Hexagonal Architecture to isolate business logic and make plugins extendable without modifying core.

Used plugin interface (IMonitorPlugin) to support scalable, open-ended integrations.

Split each functional area into bounded contexts to keep code organized and testable.

Used HttpClientFactory for testable and efficient HTTP handling in ApiSender.

⚠️ Limitations
Currently tested only on Windows due to use of PerformanceCounter and Microsoft.VisualBasic.Devices.

Disk metrics are mocked (could be extended using platform APIs).

No plugin hot-loading (plugins are statically registered at startup).

📌 Possible Improvements
Implement metrics readers for Linux/macOS using System.Diagnostics and /proc.

Add UI with WinForms/WPF or web frontend.

Add plugin auto-discovery using MEF or reflection.

📞 Contact
Created by Akshaya K R as part of the Soroco desktop engineering challenge.
Please reach out for a live demo or code walkthrough if needed.

yaml
Copy
Edit

---

Would you like this saved as a file (`README.md`) and placed in the root of your project?

Sources
