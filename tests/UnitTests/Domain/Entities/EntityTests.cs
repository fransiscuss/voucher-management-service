using Acme.Services.VoucherManagementService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Acme.Services.VoucherManagementService.UnitTests.Domain.Entities;

public class EntityTests
{
    private class TestEntity : Entity
    {
        public TestEntity()
        {
        }

        public TestEntity(string id)
        {
            Id = id;
        }
    }

    [Fact]
    public void Entity_ShouldHaveEmptyId_WhenCreatedWithDefaultConstructor()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().BeEmpty();
    }

    [Fact]
    public void Entity_ShouldHaveSpecifiedId_WhenIdIsProvided()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }
}
