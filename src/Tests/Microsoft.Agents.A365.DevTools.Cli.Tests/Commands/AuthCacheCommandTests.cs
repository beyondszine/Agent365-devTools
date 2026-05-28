// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using Microsoft.Agents.A365.DevTools.Cli.Commands;
using Microsoft.Agents.A365.DevTools.Cli.Constants;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.CommandLine;
using Xunit;

namespace Microsoft.Agents.A365.DevTools.Cli.Tests.Commands;

public class AuthCacheCommandTests : IDisposable
{
    private readonly ILogger<AuthCacheCommand> _logger = Substitute.For<ILogger<AuthCacheCommand>>();
    private readonly string _cacheDirectory = Path.Combine(Path.GetTempPath(), $"a365-auth-cache-test-{Guid.NewGuid():N}");

    public AuthCacheCommandTests()
    {
        Directory.CreateDirectory(_cacheDirectory);
    }

    [Fact]
    public async Task Clear_WhenCacheFilesExist_DeletesOnlyCliAuthTokenFile()
    {
        var authTokenPath = Path.Combine(_cacheDirectory, AuthenticationConstants.TokenCacheFileName);
        var msalTokenPath = Path.Combine(_cacheDirectory, "msal-token-cache");
        await File.WriteAllTextAsync(authTokenPath, "auth-cache");
        await File.WriteAllTextAsync(msalTokenPath, "msal-cache");

        var command = AuthCacheCommand.CreateCommand(_logger, _cacheDirectory);

        var result = await command.InvokeAsync(["clear"]);

        result.Should().Be(0);
        File.Exists(authTokenPath).Should().BeFalse();
        File.Exists(msalTokenPath).Should().BeTrue();
    }

    [Fact]
    public async Task Clear_WhenCacheFilesDoNotExist_Succeeds()
    {
        var command = AuthCacheCommand.CreateCommand(_logger, _cacheDirectory);

        var result = await command.InvokeAsync(["clear"]);

        result.Should().Be(0);
    }

    public void Dispose()
    {
        try { Directory.Delete(_cacheDirectory, recursive: true); } catch { }
    }
}
