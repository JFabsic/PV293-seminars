using Microsoft.EntityFrameworkCore;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.GetProducts;
using Yestino.ProductCatalog.Infrastructure;

namespace Yestino.ProductCatalog.Tests.Features;

public class GetProductsTests : IDisposable
{
    private readonly ProductCatalogDbContext _dbContext;

    public GetProductsTests()
    {
        var options = new DbContextOptionsBuilder<ProductCatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ProductCatalogDbContext(options, null!);
    }

    [Fact]
    public async Task GetProducts_WithNoFilter_ShouldReturnAllProducts()
    {
        // Arrange
        _dbContext.Products.AddRange(
            new Product { Name = "Product 1", Description = "Description 1", IsActive = true },
            new Product { Name = "Product 2", Description = "Description 2", IsActive = false },
            new Product { Name = "Product 3", Description = "Description 3", IsActive = true }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetProductsEndpoint.GetProducts(false, _dbContext, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainSingle(p => p.Name == "Product 1");
        result.Should().ContainSingle(p => p.Name == "Product 2");
        result.Should().ContainSingle(p => p.Name == "Product 3");
    }

    [Fact]
    public async Task GetProducts_WithOnlyActiveFilter_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        _dbContext.Products.AddRange(
            new Product { Name = "Active Product 1", Description = "Description 1", IsActive = true },
            new Product { Name = "Inactive Product", Description = "Description 2", IsActive = false },
            new Product { Name = "Active Product 2", Description = "Description 3", IsActive = true }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetProductsEndpoint.GetProducts(true, _dbContext, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(p => p.Name == "Active Product 1");
        result.Should().ContainSingle(p => p.Name == "Active Product 2");
        result.Should().NotContain(p => p.Name == "Inactive Product");
    }

    [Fact]
    public async Task GetProducts_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await GetProductsEndpoint.GetProducts(false, _dbContext, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProductsWithAllProperties()
    {
        // Arrange
        _dbContext.Products.Add(new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            ImageUrl = "https://example.com/image.jpg",
            IsActive = true
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetProductsEndpoint.GetProducts(false, _dbContext, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var product = result.First();
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    [Fact]
    public async Task GetProducts_ShouldHandleNullImageUrl()
    {
        // Arrange
        _dbContext.Products.Add(new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            ImageUrl = null,
            IsActive = true
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetProductsEndpoint.GetProducts(false, _dbContext, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().ImageUrl.Should().BeNull();
    }

    [Fact]
    public async Task GetProducts_WithOnlyActiveFilter_AndNoActiveProducts_ShouldReturnEmptyList()
    {
        // Arrange
        _dbContext.Products.AddRange(
            new Product { Name = "Inactive Product 1", Description = "Description 1", IsActive = false },
            new Product { Name = "Inactive Product 2", Description = "Description 2", IsActive = false }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetProductsEndpoint.GetProducts(true, _dbContext, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
