using System.CommandLine;
using App.Console.ParameterConfig;
using App.Core.Models.CommandModels;
using App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Console.Commands;

public static class RunRootCommand
{
    /// <summary>
    /// Add run root command
    /// </summary>
    /// <param name="rootCommand"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="ct">Cancellation token to propagate Ctrl+C</param>
    public static RootCommand AddRunCommand(this RootCommand rootCommand, IServiceProvider serviceProvider, CancellationToken ct)
    {
        rootCommand.Options.Add(RunOptions.TestOption);

        // Use external CTS token so Ctrl+C triggers cancellation across pipeline
        rootCommand.SetAction((parseResult, _) =>
            Run(new RunModel
            {
                Test = parseResult.GetValue(RunOptions.TestOption)
            }, serviceProvider, ct));

        return rootCommand;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <param name="options"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private static async Task<int> Run(RunModel options, IServiceProvider serviceProvider, CancellationToken ct)
    {
        var runSvc = serviceProvider.GetService<ProcessService>();
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory!.CreateLogger(nameof(RunRootCommand));

        try
        {
            if (ct.IsCancellationRequested)
            {
                logger.LogInformation("Cancellation requested before processing.");
                return 0;
            }

            await runSvc!.Process(options, ct);

            return 0;
        }
        catch (Exception e)
        {
            if (ct.IsCancellationRequested)
            {
                logger.LogInformation("Cancelled during processing.");

                return 0;
            }

            logger.LogError(e, "Failed to run process");

            throw;
        }
    }
}