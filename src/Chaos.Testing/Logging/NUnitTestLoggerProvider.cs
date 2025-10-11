// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Logging;

using Microsoft.Extensions.Logging;

/// <summary>
/// An <see cref="ILoggerProvider"/> that creates <see cref="NUnitTestLogger"/> instances.
/// </summary>
public sealed class NUnitTestLoggerProvider : ILoggerProvider
{
    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public ILogger CreateLogger(String _) => new NUnitTestLogger();
}
