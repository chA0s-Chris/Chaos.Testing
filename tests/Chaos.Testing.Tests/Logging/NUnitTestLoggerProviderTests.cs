// Copyright (c) 2025 Christian Flessa. All rights reserved.
// This file is licensed under the MIT license. See LICENSE in the project root for more information.
namespace Chaos.Testing.Tests.Logging;

using Chaos.Testing.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

public class NUnitTestLoggerProviderTests
{
    [Test]
    public void Constructor_ShouldCreateProvider()
    {
        // Act
        var provider = new NUnitTestLoggerProvider();

        // Assert
        provider.Should().NotBeNull();
        provider.Should().BeAssignableTo<ILoggerProvider>();
    }

    [Test]
    public void CreateLogger_AfterDispose_ShouldStillWork()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();
        provider.Dispose();

        // Act
        var logger = provider.CreateLogger("TestCategory");

        // Assert
        logger.Should().NotBeNull();
    }

    [Test]
    public void CreateLogger_ShouldReturnNUnitTestLogger()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();

        // Act
        var logger = provider.CreateLogger("TestCategory");

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeOfType<NUnitTestLogger>();
    }

    [Test]
    public void CreateLogger_WithDifferentCategories_ShouldReturnNewLoggers()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();

        // Act
        var logger1 = provider.CreateLogger("Category1");
        var logger2 = provider.CreateLogger("Category2");

        // Assert
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void CreateLogger_WithSameCategory_ShouldReturnNewLoggers()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();

        // Act
        var logger1 = provider.CreateLogger("TestCategory");
        var logger2 = provider.CreateLogger("TestCategory");

        // Assert
        logger1.Should().NotBeSameAs(logger2);
    }

    [Test]
    public void Dispose_AfterCreatingLoggers_ShouldNotThrow()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();
        provider.CreateLogger("Test1");
        provider.CreateLogger("Test2");

        // Act
        var act = () => provider.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var provider = new NUnitTestLoggerProvider();

        // Act
        var act = () => provider.Dispose();

        // Assert
        act.Should().NotThrow();
    }
}
