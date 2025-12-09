# .NET 10 Console App Template

Minimal, opinionated .NET 10 console template with:

* Dependency Injection (Microsoft.Extensions)
* Command line parsing (`System.CommandLine`)
* Structured logging (Serilog)
* Graceful Ctrl+C cancellation
* Separation of concerns via Core / Services / Console projects
* Central Package Management (Directory.Packages.props)

## Project Structure

* `App.Console` – Entry point, command + option wiring, DI + cancellation setup.
* `App.Core` – Cross‑project models (e.g. `RunModel`).
* `App.Services` – Business/service layer (`ProcessService`).

## Key Components

### Program (`App.Console/Program.cs`)
Sets up configuration, Serilog console logger, registers services, builds a `RootCommand` named `app`, wires Ctrl+C to cancellation, and invokes it with a `CancellationToken`.

### Command Extension (`RunRootCommand`)
Adds the run behavior to the root command:
* Registers option `--test` / `-t` (boolean, optional).
* Binds parsed values into `RunModel`.
* Executes `ProcessService.Process`.

### Model (`RunModel`)
```csharp
public class RunModel { public bool Test { get; set; } }
```
Represents command input passed to services.

### Service (`ProcessService`)
Logs either "Testing!" when `--test` is supplied or "Running!" otherwise.

## Features

* Clean separation between parsing, orchestration, and execution.
* Graceful shutdown on Ctrl+C (SIGINT) using `CancellationTokenSource` and `Console.CancelKeyPress`.
* Simple extension model: add new commands via extension classes similar to `RunRootCommand`.
* DI ready: register more services in `Program.cs`.
* Serilog output template kept minimal: `{Message}{NewLine}{Exception}`.

## Prerequisites

* .NET 10 SDK (C# 14). Adjust target framework if using a different SDK.

## Getting Started

1. Clone repository.
2. Restore packages: `dotnet restore`.
3. Build: `dotnet build`.
4. Run:
   * Default: `dotnet run --project App.Console`
   * Test mode: `dotnet run --project App.Console -- --test`
   * Short alias: `dotnet run --project App.Console -- -t`

Note the `--` separator before command options when using `dotnet run`.

## Command Reference

Root command name: `app`

Options:
* `--test | -t` – Simulates a test run (logs "Testing!").

Exit codes:
* `0` – Success.
* Non‑zero – Unhandled exception (logged as error).
* Cancelled – Returns success after logging "Operation cancelled." (no stack trace).

## Ctrl+C Cancellation

Pressing Ctrl+C sends SIGINT. The template:
* Intercepts the event (`Console.CancelKeyPress`).
* Prevents immediate termination (`eventArgs.Cancel = true`).
* Signals a `CancellationTokenSource`.
* Logs a friendly shutdown message.
* Catches `OperationCanceledException` and logs "Operation cancelled." before flushing logs.

Extend services/commands to honour the token by passing it into async operations.

## Logging

Configured to write to console only. Modify `Program.cs` to add sinks (e.g. file, Seq):
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    //.WriteTo.File("logs/log.txt")
    .CreateLogger();
```

## Error Handling

Exceptions inside the run pipeline are caught, logged with `logger.LogError`, and rethrown (allowing process exit with failure code for shell scripting). Cancellation is treated as a graceful path.

## License

Use/modify freely.