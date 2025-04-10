using Acme.Services.VoucherManagementService.Application.Common.PipelineBehaviours;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Application.Common.PipelineBehaviours;

public class ValidationBehaviourTests
{
    private class TestRequest
    {
        public string Name { get; set; } = "";
    }

    private class TestResponse : Result<string>
    {
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsExist()
    {
        // Arrange
        var request = new TestRequest();
        var response = new TestResponse();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(x => x()).ReturnsAsync(response);

        var behaviour = new ValidationBehavior<TestRequest, TestResponse>(new List<IValidator<TestRequest>>());

        // Act
        var result = await behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        Assert.Same(response, result);
        next.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        // Arrange
        var request = new TestRequest();
        var response = new TestResponse();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(x => x()).ReturnsAsync(response);

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behaviour = new ValidationBehavior<TestRequest, TestResponse>(validators);

        // Act
        var result = await behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        Assert.Same(response, result);
        next.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var request = new TestRequest();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        var validationFailure = new ValidationFailure("Name", "Name is required");
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { validationFailure }));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behaviour = new ValidationBehavior<TestRequest, TestResponse>(validators);

        // Act
        var result = await behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.Equal("Name is required", result.Errors[0].Message);
        next.Verify(x => x(), Times.Never);
    }
}
