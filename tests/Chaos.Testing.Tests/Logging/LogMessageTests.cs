// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Tests.Logging;

using Chaos.Testing.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

public class LogMessageTests
{
    [Test]
    public void Constructor_ShouldSetAllProperties()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var logLevel = LogLevel.Information;
        var eventId = new EventId(42, "TestEvent");
        var state = new
        {
            Id = 123
        };
        var exception = new InvalidOperationException("Test");
        var message = "Test message";

        // Act
        var logMessage = new LogMessage(timestamp, logLevel, eventId, state, exception, message);

        // Assert
        logMessage.Timestamp.Should().Be(timestamp);
        logMessage.LogLevel.Should().Be(logLevel);
        logMessage.EventId.Should().Be(eventId);
        logMessage.State.Should().BeSameAs(state);
        logMessage.Exception.Should().BeSameAs(exception);
        logMessage.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithNullState_ShouldAllowNull()
    {
        // Arrange & Act
        var logMessage = new LogMessage(
            DateTime.UtcNow,
            LogLevel.Warning,
            new(1),
            null,
            null,
            "Message");

        // Assert
        logMessage.State.Should().BeNull();
        logMessage.Exception.Should().BeNull();
    }

    [Test]
    public void Deconstruction_ShouldExtractAllProperties()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var logLevel = LogLevel.Critical;
        var eventId = new EventId(99);
        var state = "test state";
        var exception = new Exception("error");
        var message = "Critical error";
        var logMessage = new LogMessage(timestamp, logLevel, eventId, state, exception, message);

        // Act
        var (ts, level, evId, st, ex, msg) = logMessage;

        // Assert
        ts.Should().Be(timestamp);
        level.Should().Be(logLevel);
        evId.Should().Be(eventId);
        st.Should().Be(state);
        ex.Should().Be(exception);
        msg.Should().Be(message);
    }

    [Test]
    public void Equality_DifferentLogLevel_ShouldNotBeEqual()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var eventId = new EventId(1);
        var logMessage1 = new LogMessage(timestamp, LogLevel.Error, eventId, null, null, "Message");
        var logMessage2 = new LogMessage(timestamp, LogLevel.Warning, eventId, null, null, "Message");

        // Act & Assert
        logMessage1.Should().NotBe(logMessage2);
    }

    [Test]
    public void Equality_DifferentMessage_ShouldNotBeEqual()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var eventId = new EventId(1);
        var logMessage1 = new LogMessage(timestamp, LogLevel.Error, eventId, null, null, "Message1");
        var logMessage2 = new LogMessage(timestamp, LogLevel.Error, eventId, null, null, "Message2");

        // Act & Assert
        logMessage1.Should().NotBe(logMessage2);
    }

    [Test]
    public void Equality_DifferentTimestamp_ShouldNotBeEqual()
    {
        // Arrange
        var eventId = new EventId(1);
        var logMessage1 = new LogMessage(DateTime.UtcNow, LogLevel.Error, eventId, null, null, "Message");
        var logMessage2 = new LogMessage(DateTime.UtcNow.AddSeconds(1), LogLevel.Error, eventId, null, null, "Message");

        // Act & Assert
        logMessage1.Should().NotBe(logMessage2);
    }

    [Test]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var eventId = new EventId(1, "Test");
        var state = "state";
        var exception = new Exception("test");

        var logMessage1 = new LogMessage(timestamp, LogLevel.Error, eventId, state, exception, "Message");
        var logMessage2 = new LogMessage(timestamp, LogLevel.Error, eventId, state, exception, "Message");

        // Act & Assert
        logMessage1.Should().Be(logMessage2);
        (logMessage1 == logMessage2).Should().BeTrue();
    }

    [Test]
    public void GetHashCode_SameValues_ShouldHaveSameHashCode()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var eventId = new EventId(1);
        var logMessage1 = new LogMessage(timestamp, LogLevel.Error, eventId, null, null, "Message");
        var logMessage2 = new LogMessage(timestamp, LogLevel.Error, eventId, null, null, "Message");

        // Act & Assert
        logMessage1.GetHashCode().Should().Be(logMessage2.GetHashCode());
    }

    [Test]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        var timestamp = new DateTime(2025, 10, 11, 12, 0, 0, DateTimeKind.Utc);
        var eventId = new EventId(42, "TestEvent");
        var logMessage = new LogMessage(timestamp, LogLevel.Warning, eventId, null, null, "Test message");

        // Act
        var result = logMessage.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Test message");
    }
}
