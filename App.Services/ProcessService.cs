using App.Core.Models.CommandModels;
using Microsoft.Extensions.Logging;

namespace App.Services;

public interface IProcessService
{
    Task Process(RunModel options, CancellationToken token);
}

public class ProcessService(ILogger<ProcessService> logger) : IProcessService
{
    public async Task Process(RunModel options, CancellationToken token)
    {
        if (options.Test)
        {
            logger.LogInformation("Testing!");

            return;
        }

        logger.LogInformation("Running!");
    }
}