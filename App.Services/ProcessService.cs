using App.Core.Models.CommandModels;
using Microsoft.Extensions.Logging;

namespace App.Services;

public interface IProcessService
{
    /// <summary>
    /// Executes the processing operation using the specified run options.
    /// </summary>
    /// <param name="options">The options that configure the processing operation. Cannot be null.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous processing operation.</returns>
    Task Process(RunModel options, CancellationToken ct);
}

public class ProcessService(ILogger<ProcessService> logger) : IProcessService
{
    /// <inheritdoc/>
    public async Task Process(RunModel options, CancellationToken ct)
    {
        if (options.Test)
        {
            logger.LogInformation("Testing!");

            return;
        }

        logger.LogInformation("Running!");
    }
}