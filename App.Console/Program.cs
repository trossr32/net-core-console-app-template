using App.Console.Extensions;
using App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.CommandLine;

namespace App.Console;

class Program
{
    private static readonly IServiceProvider ServiceProvider;
    private static readonly IConfigurationRoot Configuration;
    private static ILogger<Program> _logger;

    static Program()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}") // log to console
            .CreateLogger();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();

        // logging
        services.AddLogging(configure => configure.AddSerilog());

        // services
        services.AddSingleton<ProcessService>();
    }

    /// <summary>
    /// App entry point. Use System.CommandLine to configure any command line arguments.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        _logger = ServiceProvider.GetService<ILogger<Program>>()!;

        var rootCommand = new RootCommand("app");

        rootCommand
            .AddRunCommand(ServiceProvider);

        await rootCommand.InvokeAsync(args);
    }

    private static void WriteLine(string text, ConsoleColor? colour = null) => WriteLine(null, text, colour);

    private static void WriteLine(Exception? e, string text, ConsoleColor? colour = null)
    {
        var oldColour = System.Console.ForegroundColor;

        if (colour.HasValue && colour != oldColour)
            System.Console.ForegroundColor = colour.Value;

        System.Console.WriteLine(text);

        if (e != null)
            System.Console.WriteLine(e.ToString());

        if (colour.HasValue && colour != oldColour)
            System.Console.ForegroundColor = oldColour;
    }
}