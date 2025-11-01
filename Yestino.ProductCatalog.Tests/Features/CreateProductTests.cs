using Microsoft.EntityFrameworkCore;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.CreateProduct;
using Yestino.ProductCatalog.Infrastructure;

namespace Yestino.ProductCatalog.Tests.Features;

public class CreateProductTests : IDisposable
{
    private readonly ProductCatalogDbContext _dbContext;

    public CreateProductTests()
    {
        var options = new DbContextOptionsBuilder<ProductCatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ProductCatalogDbContext(options, null!);
    }

    [Fact]
    public void CreateProduct_WithValidData_ShouldReturnProductIdAndRaiseEvent()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var existingProduct = CreateProductEndpoint.Before(command, _dbContext);
        var (result, storageAction, domainEvent) = CreateProductEndpoint.CreateProduct(existingProduct, command);

        // Assert - Response and storage action
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<Guid>>();
        storageAction.Should().NotBeNull();

        // Assert - Domain event
        domainEvent.Should().NotBeNull();
        domainEvent!.Name.Should().Be(command.Name);
        domainEvent.Description.Should().Be(command.Description);
        domainEvent.ImageUrl.Should().Be(command.ImageUrl);
    }

    [Fact]
    public void CreateProduct_WithoutImageUrl_ShouldSucceed()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description"
        };

        // Act
        var existingProduct = CreateProductEndpoint.Before(command, _dbContext);
        var (result, storageAction, domainEvent) = CreateProductEndpoint.CreateProduct(existingProduct, command);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<Guid>>();
        domainEvent.Should().NotBeNull();
        domainEvent!.ImageUrl.Should().BeNull();
    }

    [Fact]
    public void CreateProduct_WithNullDescription_ShouldUseEmptyString()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product"
        };

        // Act
        var existingProduct = CreateProductEndpoint.Before(command, _dbContext);
        var (result, storageAction, domainEvent) = CreateProductEndpoint.CreateProduct(existingProduct, command);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<Guid>>();
        domainEvent.Should().NotBeNull();
        domainEvent!.Description.Should().Be("");
    }

    [Fact]
    public void CreateProduct_WithDuplicateName_ShouldReturnBadRequestAndNotRaiseEvent()
    {
        // Arrange
        var existingProduct = new Product
        {
            Name = "Duplicate Product",
            Description = "Existing",
            IsActive = false
        };
        _dbContext.Products.Add(existingProduct);
        _dbContext.SaveChanges();

        var command = new CreateProductCommand
        {
            Name = "Duplicate Product",
            Description = "New Product"
        };

        // Act
        var foundProduct = CreateProductEndpoint.Before(command, _dbContext);
        var (result, storageAction, domainEvent) = CreateProductEndpoint.CreateProduct(foundProduct, command);

        // Assert - Response
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>();

        // Assert - No domain event
        domainEvent.Should().BeNull();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
