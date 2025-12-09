using App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.CommandLine;
using App.Console.Commands;

// Cancellation token source for Ctrl+C handling
var cts = new CancellationTokenSource();

var builder = new ConfigurationBuilder()
    .AddEnvironmentVariables();

builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}") // log to console
    .CreateLogger();

// Wire Ctrl+C (SIGINT) to graceful cancellation
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true; // allow graceful shutdown

    if (cts.IsCancellationRequested) 
        return;

    Log.Information("Cancellation requested (Ctrl+C). Shutting down...");
    cts.Cancel();
};

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddOptions()
    .AddLogging(configure => configure.AddSerilog())
    .AddSingleton<ProcessService>();

IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new RootCommand("app");

rootCommand
    .AddRunCommand(serviceProvider, cts.Token);

try
{
    await rootCommand.Parse(args).InvokeAsync();
}
catch (OperationCanceledException)
{
    Log.Information("Operation cancelled.");
}
finally
{
    Log.CloseAndFlush();

    if (serviceProvider is IDisposable d)
        d.Dispose();
}