using Acme.Services.VoucherManagementService.Domain.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Domain.Entities;

public class VoucherStatusTests
{
    [Fact]
    public void VoucherStatus_ShouldHaveThreeValues()
    {
        // Act
        var values = Enum.GetValues(typeof(VoucherStatus));

        // Assert
        values.Length.Should().Be(3);
        Enum.IsDefined(typeof(VoucherStatus), "Active").Should().BeTrue();
        Enum.IsDefined(typeof(VoucherStatus), "Redeemed").Should().BeTrue();
        Enum.IsDefined(typeof(VoucherStatus), "Expired").Should().BeTrue();
    }

    [Fact]
    public void VoucherStatus_ShouldSerializeAsString_WhenJsonSerialized()
    {
        // Arrange
        var status = VoucherStatus.Active;

        // Act
        var serialized = JsonConvert.SerializeObject(status);

        // Assert
        serialized.Should().Be("\"Active\"");
    }
}
