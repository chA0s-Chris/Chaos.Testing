// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Tests.Logging;

using Chaos.Testing.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;

public class NUnitTestLoggerTests
{
    [Test]
    public void BeginScope_Dispose_ShouldNotThrow()
    {
        // Arrange
        var logger = new NUnitTestLogger();
        var scope = logger.BeginScope("test scope");

        // Act
        var act = () => scope?.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void BeginScope_ShouldReturnDisposable()
    {
        // Arrange
        var logger = new NUnitTestLogger();

        // Act
        var scope = logger.BeginScope("test scope");

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Test]
    public void Constructor_WithCaptureMessagesTrue_ShouldEnableCapture()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();

        // Act
        var logger = new NUnitTestLogger(true, timeProvider);

        // Assert
        logger.Should().NotBeNull();
        logger.LogMessages.Should().BeEmpty();
    }

    [Test]
    public void Constructor_WithoutParameters_ShouldCreateLogger()
    {
        // Act
        var logger = new NUnitTestLogger();

        // Assert
        logger.Should().NotBeNull();
        logger.LogMessages.Should().BeEmpty();
    }

    [Test]
    public void GenericLogger_ShouldUseCategoryType()
    {
        // Arrange & Act
        var logger = new NUnitTestLogger<NUnitTestLoggerTests>(true);

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeAssignableTo<ILogger<NUnitTestLoggerTests>>();
    }

    [Test]
    public void IsEnabled_WithLogLevelNone_ShouldReturnFalse()
    {
        // Arrange
        var logger = new NUnitTestLogger();

        // Act
        var result = logger.IsEnabled(LogLevel.None);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    [TestCase(LogLevel.Trace)]
    [TestCase(LogLevel.Debug)]
    [TestCase(LogLevel.Information)]
    [TestCase(LogLevel.Warning)]
    [TestCase(LogLevel.Error)]
    [TestCase(LogLevel.Critical)]
    public void IsEnabled_WithValidLogLevel_ShouldReturnTrue(LogLevel logLevel)
    {
        // Arrange
        var logger = new NUnitTestLogger();

        // Act
        var result = logger.IsEnabled(logLevel);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Log_MultipleTimes_ShouldCaptureAllMessages()
    {
        // Arrange
        var logger = new NUnitTestLogger(true);

        // Act
        logger.LogTrace("Trace message");
        logger.LogDebug("Debug message");
        logger.LogInformation("Info message");
        logger.LogWarning("Warning message");
        logger.LogError("Error message");
        logger.LogCritical("Critical message");

        // Assert
        logger.LogMessages.Should().HaveCount(6);
        logger.LogMessages[0].LogLevel.Should().Be(LogLevel.Trace);
        logger.LogMessages[1].LogLevel.Should().Be(LogLevel.Debug);
        logger.LogMessages[2].LogLevel.Should().Be(LogLevel.Information);
        logger.LogMessages[3].LogLevel.Should().Be(LogLevel.Warning);
        logger.LogMessages[4].LogLevel.Should().Be(LogLevel.Error);
        logger.LogMessages[5].LogLevel.Should().Be(LogLevel.Critical);
    }

    [Test]
    public void Log_WithCaptureDisabled_ShouldNotCaptureMessage()
    {
        // Arrange
        var logger = new NUnitTestLogger(false);

        // Act
        logger.LogInformation("Test message");

        // Assert
        logger.LogMessages.Should().BeEmpty();
    }

    [Test]
    public void Log_WithCaptureEnabled_ShouldCaptureMessage()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider(new(2025, 10, 11, 12, 0, 0, TimeSpan.Zero));
        var logger = new NUnitTestLogger(true, timeProvider);
        var eventId = new EventId(42, "TestEvent");

        // Act
        logger.LogInformation(eventId, "Test message");

        // Assert
        logger.LogMessages.Should().HaveCount(1);
        var captured = logger.LogMessages[0];
        captured.Message.Should().Be("Test message");
        captured.LogLevel.Should().Be(LogLevel.Information);
        captured.EventId.Should().Be(eventId);
        captured.Timestamp.Should().Be(timeProvider.GetUtcNow().UtcDateTime);
    }

    [Test]
    public void Log_WithEmptyMessage_ShouldNotCaptureOrLog()
    {
        // Arrange
        var logger = new NUnitTestLogger(true);

        // Act
        logger.Log(LogLevel.Information, new(1), "state", null, (_, _) => String.Empty);

        // Assert
        logger.LogMessages.Should().BeEmpty();
    }

    [Test]
    public void Log_WithException_ShouldCaptureException()
    {
        // Arrange
        var logger = new NUnitTestLogger(true);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.LogError(exception, "Error occurred");

        // Assert
        logger.LogMessages.Should().HaveCount(1);
        var captured = logger.LogMessages[0];
        captured.Message.Should().Be("Error occurred");
        captured.Exception.Should().BeSameAs(exception);
    }

    [Test]
    public void Log_WithNullFormatter_ShouldThrowArgumentNullException()
    {
        // Arrange
        var logger = new NUnitTestLogger();

        // Act
        var act = () => logger.Log(LogLevel.Information, new(1), "state", null, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("formatter");
    }

    [Test]
    public void Log_WithNullMessage_ShouldNotCaptureOrLog()
    {
        // Arrange
        var logger = new NUnitTestLogger(true);

        // Act
        logger.Log(LogLevel.Information, new(1), "state", null, (_, _) => null!);

        // Assert
        logger.LogMessages.Should().BeEmpty();
    }

    [Test]
    public void Log_WithState_ShouldCaptureState()
    {
        // Arrange
        var logger = new NUnitTestLogger(true);
        var state = new
        {
            Id = 123,
            Name = "Test"
        };

        // Act
        logger.Log(LogLevel.Information, new(1), state, null, (s, _) => $"Id: {s.Id}, Name: {s.Name}");

        // Assert
        logger.LogMessages.Should().HaveCount(1);
        var captured = logger.LogMessages[0];
        captured.State.Should().BeSameAs(state);
        captured.Message.Should().Be("Id: 123, Name: Test");
    }

    [Test]
    public void TimeProvider_ShouldBeUsedForTimestamps()
    {
        // Arrange
        var baseTime = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(baseTime);
        var logger = new NUnitTestLogger(true, timeProvider);

        // Act
        logger.LogInformation("Message 1");
        timeProvider.Advance(TimeSpan.FromHours(1));
        logger.LogInformation("Message 2");
        timeProvider.Advance(TimeSpan.FromHours(1));
        logger.LogInformation("Message 3");

        // Assert
        logger.LogMessages.Should().HaveCount(3);
        logger.LogMessages[0].Timestamp.Should().Be(baseTime.UtcDateTime);
        logger.LogMessages[1].Timestamp.Should().Be(baseTime.AddHours(1).UtcDateTime);
        logger.LogMessages[2].Timestamp.Should().Be(baseTime.AddHours(2).UtcDateTime);
    }
}
