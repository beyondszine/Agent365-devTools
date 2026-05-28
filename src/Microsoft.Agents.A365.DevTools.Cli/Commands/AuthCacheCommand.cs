// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Agents.A365.DevTools.Cli.Constants;
using Microsoft.Agents.A365.DevTools.Cli.Services;
using Microsoft.Extensions.Logging;

namespace Microsoft.Agents.A365.DevTools.Cli.Commands;

public class AuthCacheCommand
{
    public static Command CreateCommand(
        ILogger<AuthCacheCommand> logger,
        string? cacheDirectory = null)
    {
        var authCacheCommand = new Command(CommandNames.AuthCache, "Manage cached authentication tokens");
        authCacheCommand.AddCommand(CreateClearCommand(logger, cacheDirectory));
        return authCacheCommand;
    }

    private static Command CreateClearCommand(
        ILogger<AuthCacheCommand> logger,
        string? cacheDirectory)
    {
        var clearCommand = new Command("clear", "Clear the Agent 365 CLI authentication token cache");

        clearCommand.SetHandler((InvocationContext context) =>
        {
            var resolvedCacheDirectory = cacheDirectory ?? ConfigService.GetGlobalConfigDirectory();
            var path = Path.Combine(resolvedCacheDirectory, AuthenticationConstants.TokenCacheFileName);
            try
            {
                if (!File.Exists(path))
                {
                    logger.LogInformation("Not found: {Path}", path);
                    logger.LogInformation("No Agent 365 CLI authentication cache file was found.");
                    return;
                }

                File.Delete(path);
                logger.LogInformation("Deleted: {Path}", path);
                logger.LogInformation("Cleared Agent 365 CLI authentication cache.");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
            {
                logger.LogError("Failed to delete {Path}: {Message}", path, ex.Message);
                context.ExitCode = 1;
            }
        });

        return clearCommand;
    }
}
