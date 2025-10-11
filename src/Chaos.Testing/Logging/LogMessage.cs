// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Logging;

using Microsoft.Extensions.Logging;

/// <summary>
/// Represents a captured log message with associated metadata.
/// </summary>
/// <param name="Timestamp">The UTC timestamp when the log message was created.</param>
/// <param name="LogLevel">The severity level of the log message.</param>
/// <param name="EventId">The event identifier associated with the log message.</param>
/// <param name="State">The state object passed to the logger.</param>
/// <param name="Exception">The exception associated with the log message, if any.</param>
/// <param name="Message">The formatted log message text.</param>
public record LogMessage(
    DateTime Timestamp,
    LogLevel LogLevel,
    EventId EventId,
    Object? State,
    Exception? Exception,
    String Message);
