using System.CommandLine;

namespace App.Console.ParameterConfig;

internal static class RunOptions
{
    internal static readonly Option<bool> TestOption = new(aliases: ["-t", "--test"])
    {
        Description = "Simulates a test run",
        IsRequired = false
    };
}