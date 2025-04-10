using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Application.Models.Common;

public class ValidationErrorTests
{
    private enum TestErrorCode
    {
        Error1,
        Error2
    }

    [Fact]
    public void ValidationError_ShouldSetMessageAndErrorCode()
    {
        // Arrange
        var message = "Validation failed";
        var errorCode = TestErrorCode.Error1;

        // Act
        var error = new ValidationError(message, errorCode);

        // Assert
        error.Message.Should().Be(message);
        error.Metadata.Should().ContainKey("ErrorCode");
        error.Metadata["ErrorCode"].Should().Be(errorCode);
    }
}
