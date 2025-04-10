using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Application.Common.Extensions;

public class EnumerableExtensionsTests
{
    [Fact]
    public void ToUserDefinedProperties_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange
        List<UserDefinedProperty>? input = null;

        // Act
        var result = input.ToUserDefinedProperties();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToUserDefinedProperties_ShouldReturnNull_WhenInputIsEmpty()
    {
        // Arrange
        var input = new List<UserDefinedProperty>();

        // Act
        var result = input.ToUserDefinedProperties();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToUserDefinedProperties_ShouldMapCorrectly_WhenInputIsValid()
    {
        // Arrange
        var input = new List<UserDefinedProperty>
        {
            new() { Id = "test1", Values = new List<string> { "value1", "value2" } },
            new() { Id = "test2", Values = new List<string> { "value3" } }
        };

        // Act
        var result = input.ToUserDefinedProperties();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().Be(2);
        
        result[0].Id.Should().Be("test1");
        result[0].Values.Should().BeEquivalentTo(new List<string> { "value1", "value2" });
        
        result[1].Id.Should().Be("test2");
        result[1].Values.Should().BeEquivalentTo(new List<string> { "value3" });
    }

    [Fact]
    public void ToUserDefinedPropertyModels_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange
        List<Acme.Services.VoucherManagementService.Domain.Entities.UserDefinedProperty>? input = null;

        // Act
        var result = input.ToUserDefinedPropertyModels();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToUserDefinedPropertyModels_ShouldReturnNull_WhenInputIsEmpty()
    {
        // Arrange
        var input = new List<Acme.Services.VoucherManagementService.Domain.Entities.UserDefinedProperty>();

        // Act
        var result = input.ToUserDefinedPropertyModels();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToUserDefinedPropertyModels_ShouldMapCorrectly_WhenInputIsValid()
    {
        // Arrange
        var input = new List<Acme.Services.VoucherManagementService.Domain.Entities.UserDefinedProperty>
        {
            new() { Id = "test1", Values = new List<string> { "value1", "value2" } },
            new() { Id = "test2", Values = new List<string> { "value3" } }
        };

        // Act
        var result = input.ToUserDefinedPropertyModels();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().Be(2);
        
        result[0].Id.Should().Be("test1");
        result[0].Values.Should().BeEquivalentTo(new List<string> { "value1", "value2" });
        
        result[1].Id.Should().Be("test2");
        result[1].Values.Should().BeEquivalentTo(new List<string> { "value3" });
    }
}
