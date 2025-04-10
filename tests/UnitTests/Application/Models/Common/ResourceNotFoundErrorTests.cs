using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Application.Models.Common;

public class ResourceNotFoundErrorTests
{
    [Fact]
    public void ResourceNotFoundError_ShouldSetMessage()
    {
        // Arrange
        var message = "Resource not found";

        // Act
        var error = new ResourceNotFoundError(message);

        // Assert
        error.Message.Should().Be(message);
    }
}
