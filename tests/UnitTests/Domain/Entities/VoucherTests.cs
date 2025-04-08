using Acme.Services.VoucherManagementService.Domain.Entities;
using FluentAssertions;
using FluentResults;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Domain.Entities;

public class VoucherTests
{
    [Fact]
    public void Voucher_ShouldBeCreatedCorrectly()
    {
        // Arrange
        var issuingParty = "ACME";
        var category = "Promotion";
        var batchNumber = "BATCH-2023-01";
        var code = "PROMO123";
        var status = VoucherStatus.Active;
        var expiryDate = DateTime.UtcNow.AddDays(30).Date;
        var userDefinedProperties = new List<UserDefinedProperty>
        {
            new() { Id = "discount", Values = new List<string> { "10%" } }
        };

        // Act
        var voucher = new Voucher(
            issuingParty,
            category,
            batchNumber,
            code,
            status,
            expiryDate,
            userDefinedProperties
        );

        // Assert
        voucher.IssuingParty.Should().Be(issuingParty);
        voucher.Category.Should().Be(category);
        voucher.BatchNumber.Should().Be(batchNumber);
        voucher.Code.Should().Be(code);
        voucher.Status.Should().Be(status);
        voucher.ExpiryDateUtc.Should().Be(expiryDate);
        voucher.UserDefinedProperties.Should().BeEquivalentTo(userDefinedProperties);
        voucher.Id.Should().NotBeEmpty();
        voucher.CreatedDateUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Status_ShouldReturnExpired_WhenExpiryDateIsPassed()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Active,
            DateTime.UtcNow.AddDays(-1).Date,
            null
        );

        // Act & Assert
        voucher.Status.Should().Be(VoucherStatus.Expired);
    }

    [Fact]
    public void Status_ShouldReturnRedeemed_WhenVoucherIsRedeemed()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Redeemed,
            DateTime.UtcNow.AddDays(30).Date,
            null
        );

        // Act & Assert
        voucher.Status.Should().Be(VoucherStatus.Redeemed);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenVoucherIsActive()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Active,
            DateTime.UtcNow.AddDays(30).Date,
            null
        );

        // Act
        var result = voucher.Validate();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenVoucherIsExpired()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Active,
            DateTime.UtcNow.AddDays(-1).Date,
            null
        );

        // Act
        var result = voucher.Validate();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Single().Message.Should().Be("Voucher is expired");
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenVoucherIsRedeemed()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Redeemed,
            DateTime.UtcNow.AddDays(30).Date,
            null
        );

        // Act
        var result = voucher.Validate();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Single().Message.Should().Be("Voucher is already redeemed");
    }

    [Fact]
    public void Redeem_ShouldMarkVoucherAsRedeemed_WhenVoucherIsActive()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Active,
            DateTime.UtcNow.AddDays(30).Date,
            null
        );

        var redemptionId = "order-123";
        var redemptionType = "Order";

        // Act
        var result = voucher.Redeem(redemptionId, redemptionType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        voucher.Status.Should().Be(VoucherStatus.Redeemed);
        voucher.RedemptionId.Should().Be(redemptionId);
        voucher.RedemptionType.Should().Be(redemptionType);
        voucher.RedeemedDateUtc.Should().NotBeNull();
        voucher.RedeemedDateUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Redeem_ShouldReturnFailure_WhenVoucherIsExpired()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Active,
            DateTime.UtcNow.AddDays(-1).Date,
            null
        );

        // Act
        var result = voucher.Redeem("order-123", "Order");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Single().Message.Should().Be("Voucher is expired");
        voucher.Status.Should().Be(VoucherStatus.Expired);
        voucher.RedemptionId.Should().BeNull();
        voucher.RedemptionType.Should().BeNull();
        voucher.RedeemedDateUtc.Should().BeNull();
    }

    [Fact]
    public void Redeem_ShouldReturnFailure_WhenVoucherIsAlreadyRedeemed()
    {
        // Arrange
        var voucher = new Voucher(
            "ACME",
            "Promotion",
            "BATCH-2023-01",
            "PROMO123",
            VoucherStatus.Redeemed,
            DateTime.UtcNow.AddDays(30).Date,
            null
        );
        voucher.Redeem("old-order", "Order");

        // Act
        var result = voucher.Redeem("new-order", "Order");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Single().Message.Should().Be("Voucher is already redeemed");
    }
}
