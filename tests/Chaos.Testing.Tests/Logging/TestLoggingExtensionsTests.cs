// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Tests.Logging;

using Chaos.Testing.Logging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

public class TestLoggingExtensionsTests
{
    [Test]
    public void AddNUnit_CalledMultipleTimes_ShouldRegisterOnlyOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddNUnit();
            builder.AddNUnit();
            builder.AddNUnit();
        });

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>()
                                             .Where(p => p is NUnitTestLoggerProvider)
                                             .ToList();

        // Assert
        loggerProviders.Should().HaveCount(1);
    }

    [Test]
    public void AddNUnit_ShouldRegisterLoggerProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddNUnit());

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>();

        // Assert
        loggerProviders.Should().Contain(p => p is NUnitTestLoggerProvider);
    }

    [Test]
    public void AddNUnit_ShouldReturnLoggingBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        ILoggingBuilder? capturedBuilder = null;

        // Act
        services.AddLogging(builder =>
        {
            var result = builder.AddNUnit();
            capturedBuilder = result;
        });

        // Assert
        capturedBuilder.Should().NotBeNull();
        capturedBuilder.Should().BeAssignableTo<ILoggingBuilder>();
    }

    [Test]
    public void AddNUnitTestLogging_CreatedLogger_ShouldBeNUnitTestLogger()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddNUnitTestLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger("TestCategory");

        // Assert
        logger.Should().NotBeNull();
    }

    [Test]
    public void AddNUnitTestLogging_ShouldAllowLoggingToWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddNUnitTestLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger<TestLoggingExtensionsTests>();

        // Assert
        logger.Should().NotBeNull();
        logger.IsEnabled(LogLevel.Information).Should().BeTrue();
    }

    [Test]
    public void AddNUnitTestLogging_ShouldRegisterLoggingServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNUnitTestLogging();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        loggerFactory.Should().NotBeNull();
    }

    [Test]
    public void AddNUnitTestLogging_ShouldRegisterNUnitLoggerProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNUnitTestLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>();

        // Assert
        loggerProviders.Should().Contain(p => p is NUnitTestLoggerProvider);
    }

    [Test]
    public void AddNUnitTestLogging_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddNUnitTestLogging();

        // Assert
        result.Should().BeSameAs(services);
    }
}
