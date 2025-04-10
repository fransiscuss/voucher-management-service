using Acme.Services.VoucherManagementService.Application.Common.PipelineBehaviours;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Application.Common.PipelineBehaviours;

public class LoggingBehaviourTests
{
    private class TestRequest
    {
        public string Name { get; set; } = "";
    }

    private class TestSensitiveRequest : IContainsSensitiveData
    {
        public string SecretValue { get; set; } = "";
    }

    private class TestResponse
    {
    }

    private readonly Mock<ILogger<LoggingBehaviour<TestRequest, TestResponse>>> _logger;

    public LoggingBehaviourTests()
    {
        _logger = new Mock<ILogger<LoggingBehaviour<TestRequest, TestResponse>>>();
    }

    [Fact]
    public async Task Handle_ShouldLogRequestAndCallNext()
    {
        // Arrange
        var request = new TestRequest { Name = "Test" };
        var response = new TestResponse();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(x => x()).ReturnsAsync(response);

        var behaviour = new LoggingBehaviour<TestRequest, TestResponse>(_logger.Object);

        // Act
        var result = await behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(response);
        next.Verify(x => x(), Times.Once);
        _logger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotSerializeRequest_WhenContainsSensitiveData()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggingBehaviour<TestSensitiveRequest, TestResponse>>>();
        var request = new TestSensitiveRequest { SecretValue = "Secret" };
        var response = new TestResponse();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(x => x()).ReturnsAsync(response);

        var behaviour = new LoggingBehaviour<TestSensitiveRequest, TestResponse>(loggerMock.Object);

        // Act
        var result = await behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(response);
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[Contains sensitive data]")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
