using Acme.Services.VoucherManagementService.Domain.Messaging.ServiceBus;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Domain.Messaging.ServiceBus;

public class VoucherRedeemMessageTests
{
    [Fact]
    public void VoucherRedeemMessage_ShouldBeCreatedCorrectly()
    {
        // Arrange
        var code = "PROMO123";
        var status = "Redeemed";
        var issuingParty = "ACME";
        var category = "Promotion";
        var batchNumber = "BATCH-2023-01";
        var redemptionId = "order-123";
        var redemptionType = "Order";
        var expiryDateUtc = DateTime.UtcNow.Date;
        var userDefinedProperties = new List<UserDefinedProperty>
        {
            new("discount", new List<string> { "10%" })
        };

        // Act
        var message = new VoucherRedeemMessage(
            code,
            status,
            issuingParty,
            category,
            batchNumber,
            redemptionId,
            redemptionType,
            expiryDateUtc,
            userDefinedProperties
        );

        // Assert
        message.Code.Should().Be(code);
        message.Status.Should().Be(status);
        message.IssuingParty.Should().Be(issuingParty);
        message.Category.Should().Be(category);
        message.BatchNumber.Should().Be(batchNumber);
        message.RedemptionId.Should().Be(redemptionId);
        message.RedemptionType.Should().Be(redemptionType);
        message.ExpiryDateUtc.Should().Be(expiryDateUtc);
        message.UserDefinedProperties.Should().HaveCount(1);
        message.UserDefinedProperties!.First().Id.Should().Be("discount");
        message.UserDefinedProperties!.First().Values.Should().BeEquivalentTo(new List<string> { "10%" });
    }

    [Fact]
    public void UserDefinedProperty_ShouldBeCreatedCorrectly()
    {
        // Arrange
        var id = "discount";
        var values = new List<string> { "10%" };

        // Act
        var property = new UserDefinedProperty(id, values);

        // Assert
        property.Id.Should().Be(id);
        property.Values.Should().BeEquivalentTo(values);
    }
}
