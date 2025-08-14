using App.Console.Extensions;
using App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.CommandLine;

var builder = new ConfigurationBuilder()
    .AddEnvironmentVariables();

builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}") // log to console
    .CreateLogger();

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddOptions()
    .AddLogging(configure => configure.AddSerilog())
    .AddSingleton<ProcessService>();

IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new RootCommand("app");

rootCommand
    .AddRunCommand(serviceProvider);

await rootCommand.InvokeAsync(args);