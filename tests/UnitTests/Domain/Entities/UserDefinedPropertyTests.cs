using Acme.Services.VoucherManagementService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Domain.Entities;

public class UserDefinedPropertyTests
{
    [Fact]
    public void UserDefinedProperty_ShouldInitializeWithEmptyValues()
    {
        // Arrange & Act
        var property = new UserDefinedProperty();

        // Assert
        property.Id.Should().BeEmpty();
        property.Values.Should().NotBeNull();
        property.Values.Should().BeEmpty();
    }

    [Fact]
    public void UserDefinedProperty_ShouldSetIdAndValues()
    {
        // Arrange
        var id = "customField";
        var values = new List<string> { "value1", "value2" };

        // Act
        var property = new UserDefinedProperty
        {
            Id = id,
            Values = values
        };

        // Assert
        property.Id.Should().Be(id);
        property.Values.Should().BeEquivalentTo(values);
    }
}
