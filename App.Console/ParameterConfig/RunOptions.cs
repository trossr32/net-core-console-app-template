using System.CommandLine;

namespace App.Console.ParameterConfig;

internal static class RunOptions
{
    /// <summary>
    /// Represents a command-line option that enables simulation of a test run when specified.
    /// </summary>
    /// <remarks>This option can be specified using either "--test" or its alias "-t". It is optional and,
    /// when provided, indicates that the operation should be performed in test mode without making permanent
    /// changes.</remarks>
    internal static readonly Option<bool> TestOption = new("--test")
    {
        Aliases = { "-t" },
        Description = "Simulates a test run",
        Required = false
    };
}