// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Logging;

using Microsoft.Extensions.Logging;

/// <summary>
/// An <see cref="ILogger{T}"/> implementation that writes log messages to the NUnit test output
/// via <see cref="NUnit.Framework.TestContext.Progress"/>. Optionally captures log messages for assertions.
/// </summary>
/// <typeparam name="T">The type whose name is used as the logger category.</typeparam>
public class NUnitTestLogger<T> : ILogger<T>
{
    private readonly Boolean _captureMessages;
    private readonly List<LogMessage> _messages = [];
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="NUnitTestLogger{T}"/> class.
    /// </summary>
    /// <param name="captureMessages">If <c>true</c>, log messages will be captured in <see cref="LogMessages"/> for later inspection.</param>
    /// <param name="timeProvider">The time provider to use for timestamping captured messages. Defaults to <see cref="TimeProvider.System"/>.</param>
    public NUnitTestLogger(Boolean captureMessages = false, TimeProvider? timeProvider = null)
    {
        _captureMessages = captureMessages;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Gets the collection of captured log messages. Only populated when <c>captureMessages</c> is <c>true</c> in the constructor.
    /// </summary>
    public IReadOnlyList<LogMessage> LogMessages => _messages;

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => DummyDisposable.Instance;

    /// <inheritdoc/>
    public Boolean IsEnabled(LogLevel logLevel)
        => NUnit.Framework.TestContext.Progress is not null &&
           logLevel != LogLevel.None;

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatter"/> is <c>null</c>.</exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, String> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter.Invoke(state, exception);
        if (String.IsNullOrEmpty(message))
        {
            return;
        }

        if (_captureMessages)
        {
            var logMessage = new LogMessage(_timeProvider.GetUtcNow().UtcDateTime, logLevel, eventId, state, exception, message);
            _messages.Add(logMessage);
        }

        var output = $"{logLevel}: {message}";
        if (exception is not null)
        {
            output += $"{Environment.NewLine}{exception}";
        }

        NUnit.Framework.TestContext.Progress.WriteLine(output);
    }

    private class DummyDisposable : IDisposable
    {
        public static DummyDisposable Instance => new();

        public void Dispose() { }
    }
}

/// <summary>
/// A non-generic version of <see cref="NUnitTestLogger{T}"/> that uses itself as the category type.
/// </summary>
public sealed class NUnitTestLogger : NUnitTestLogger<NUnitTestLogger>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NUnitTestLogger"/> class.
    /// </summary>
    /// <param name="captureMessages">If <c>true</c>, log messages will be captured in <see cref="NUnitTestLogger{T}.LogMessages"/> for later inspection.</param>
    /// <param name="timeProvider">The time provider to use for timestamping captured messages. Defaults to <see cref="TimeProvider.System"/>.</param>
    public NUnitTestLogger(Boolean captureMessages = false, TimeProvider? timeProvider = null)
        : base(captureMessages, timeProvider) { }
}
