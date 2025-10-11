// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods for configuring NUnit test logging.
/// </summary>
public static class TestLoggingExtensions
{
    /// <summary>
    /// Adds the NUnit logger provider to the logging builder.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to configure.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> for method chaining.</returns>
    public static ILoggingBuilder AddNUnit(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NUnitTestLoggerProvider>());
        return builder;
    }

    /// <summary>
    /// Adds logging services configured with the NUnit logger provider to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddNUnitTestLogging(this IServiceCollection services)
        => services.AddLogging(builder => builder.AddNUnit());
}
