# Chaos.Testing

**Chaos.Testing** is a lightweight .NET library that provides testing helpers for NUnit tests. It integrates seamlessly with `Microsoft.Extensions.Logging` to write test logs to the NUnit test output
and optionally capture log messages for assertions.

[![GitHub License](https://img.shields.io/github/license/chA0s-Chris/Chaos.Testing?style=for-the-badge)](https://github.com/chA0s-Chris/Chaos.Testing/blob/main/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/Chaos.Testing?style=for-the-badge)](https://www.nuget.org/packages/Chaos.Testing)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Chaos.Testing?style=for-the-badge)](https://www.nuget.org/packages/Chaos.Testing)
[![GitHub last commit](https://img.shields.io/github/last-commit/chA0s-Chris/Chaos.Testing?style=for-the-badge)](https://github.com/chA0s-Chris/Chaos.Testing/commits/)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/chA0s-Chris/Chaos.Testing/ci.yml?style=for-the-badge)]()

## Features

- **NUnit Integration**: Write logs directly to NUnit's `TestContext.Progress` output
- **Log Capture**: Optionally capture log messages for assertions in your tests
- **TimeProvider Support**: Full support for `TimeProvider` abstraction, enabling deterministic timestamp testing
- **Generic and Non-Generic Loggers**: Use `NUnitTestLogger<T>` or `NUnitTestLogger` depending on your needs
- **Dependency Injection**: Easy integration with `Microsoft.Extensions.DependencyInjection` and `ILoggingBuilder`
- **Structured Logging**: Capture complete log metadata including state, exceptions, event IDs, and timestamps

## Installation

Install the package via NuGet:

```bash
dotnet add package Chaos.Testing
```

Or via the Package Manager Console:

```powershell
Install-Package Chaos.Testing
```

## Usage

### Basic Logger Usage

```csharp
using Chaos.Testing.Logging;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

public class MyTests
{
    [Test]
    public void MyTest()
    {
        // Create a logger that writes to NUnit test output
        var logger = new NUnitTestLogger();

        logger.LogInformation("This message appears in test output");
        logger.LogWarning("Warning message");
        logger.LogError("Error message");
    }
}
```

### Capturing Log Messages for Assertions

```csharp
[Test]
public void MyTest_WithLogCapture()
{
    // Enable log capture by passing true to the constructor
    var logger = new NUnitTestLogger(captureMessages: true);

    // Log some messages
    logger.LogInformation("User logged in");
    logger.LogWarning("Cache miss");

    // Assert on captured log messages
    Assert.That(logger.LogMessages, Has.Count.EqualTo(2));
    Assert.That(logger.LogMessages[0].LogLevel, Is.EqualTo(LogLevel.Information));
    Assert.That(logger.LogMessages[0].Message, Is.EqualTo("User logged in"));
    Assert.That(logger.LogMessages[1].LogLevel, Is.EqualTo(LogLevel.Warning));
}
```

### Using with TimeProvider for Deterministic Testing

```csharp
using Microsoft.Extensions.Time.Testing;

[Test]
public void MyTest_WithFakeTime()
{
    var fakeTime = new FakeTimeProvider(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    var logger = new NUnitTestLogger(captureMessages: true, timeProvider: fakeTime);

    logger.LogInformation("First message");

    // Advance time
    fakeTime.Advance(TimeSpan.FromHours(1));
    logger.LogInformation("Second message");

    // Assert on timestamps
    Assert.That(logger.LogMessages[0].Timestamp, Is.EqualTo(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    Assert.That(logger.LogMessages[1].Timestamp, Is.EqualTo(new DateTime(2025, 1, 1, 1, 0, 0, DateTimeKind.Utc)));
}
```

### Generic Logger with Type Categories

```csharp
[Test]
public void MyTest_WithGenericLogger()
{
    var logger = new NUnitTestLogger<MyTests>(captureMessages: true);

    // Logger category is automatically set to the type name
    logger.LogInformation("Message from MyTests");

    Assert.That(logger, Is.InstanceOf<ILogger<MyTests>>());
}
```

### Dependency Injection Integration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Chaos.Testing.Logging;

[Test]
public void MyTest_WithDependencyInjection()
{
    var services = new ServiceCollection();

    // Add NUnit test logging to your service collection
    services.AddNUnitTestLogging();

    // OR add it directly to an ILoggingBuilder
    services.AddLogging(builder => builder.AddNUnit());

    var serviceProvider = services.BuildServiceProvider();
    var logger = serviceProvider.GetRequiredService<ILogger<MyTests>>();

    logger.LogInformation("Logging from dependency injection");
}
```

### Capturing Structured State

```csharp
[Test]
public void MyTest_WithStructuredState()
{
    var logger = new NUnitTestLogger(captureMessages: true);
    var state = new { UserId = 123, Action = "Login" };

    logger.Log(
        LogLevel.Information,
        new EventId(1, "UserAction"),
        state,
        null,
        (s, ex) => $"User {s.UserId} performed {s.Action}"
    );

    var captured = logger.LogMessages[0];
    Assert.That(captured.State, Is.SameAs(state));
    Assert.That(captured.Message, Is.EqualTo("User 123 performed Login"));
    Assert.That(captured.EventId.Name, Is.EqualTo("UserAction"));
}
```

## API Reference

### NUnitTestLogger

The main logger implementation.

**Constructor Parameters:**

- `captureMessages` (Boolean, optional): If `true`, log messages are captured in the `LogMessages` collection. Default: `false`
- `timeProvider` (TimeProvider?, optional): The time provider used for timestamping captured messages. Default: `TimeProvider.System`

**Properties:**

- `LogMessages` (IReadOnlyList\<LogMessage\>): Collection of captured log messages (only populated when `captureMessages` is `true`)

### NUnitTestLogger\<T\>

Generic version of the logger with typed category.

### LogMessage

Record representing a captured log message.

**Properties:**

- `Timestamp` (DateTime): UTC timestamp when the message was logged
- `LogLevel` (LogLevel): Severity level of the log message
- `EventId` (EventId): Event identifier associated with the log message
- `State` (Object?): State object passed to the logger
- `Exception` (Exception?): Exception associated with the log message, if any
- `Message` (String): Formatted log message text

### Extension Methods

#### ILoggingBuilder.AddNUnit()

Adds the NUnit logger provider to the logging builder.

#### IServiceCollection.AddNUnitTestLogging()

Adds logging services configured with the NUnit logger provider.

## Requirements

- **.NET 8.0** or **.NET 9.0**
- **NUnit** testing framework
- **Microsoft.Extensions.Logging** and **Microsoft.Extensions.Logging.Abstractions**

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests on [GitHub](https://github.com/chA0s-Chris/Chaos.Testing).

## License

MIT License - see [LICENSE](./LICENSE) for more information.

Copyright © 2025 Christian Flessa
