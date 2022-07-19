using Microsoft.Extensions.Logging;

namespace App.Services;

public interface IProcessService
{
    Task Process(bool isTest = false);
}

public class ProcessService : IProcessService
{
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(ILogger<ProcessService> logger)
    {
        _logger = logger;
    }

    public async Task Process(bool isTest = false)
    {
        if (isTest)
            _logger?.LogInformation("Testing!");
    }
}