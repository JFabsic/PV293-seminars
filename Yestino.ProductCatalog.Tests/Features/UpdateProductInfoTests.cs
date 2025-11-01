using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.UpdateProductInfo;

namespace Yestino.ProductCatalog.Tests.Features;

public class UpdateProductInfoTests
{
    [Fact]
    public void UpdateProductInfo_ShouldUpdateAllPropertiesAndPreserveIsActive()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            ImageUrl = "https://example.com/original.jpg",
            IsActive = true
        };

        var command = new UpdateProductInfoCommand(
            "Updated Name",
            "Updated Description",
            "https://example.com/updated.jpg"
        );

        // Act
        var storageAction = UpdateProductInfoEndpoint.UpdateProductInfo(product, command);

        // Assert - Updated properties
        product.Name.Should().Be("Updated Name");
        product.Description.Should().Be("Updated Description");
        product.ImageUrl.Should().Be("https://example.com/updated.jpg");

        // Assert - Preserved property
        product.IsActive.Should().BeTrue();

        // Assert - Storage action
        storageAction.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProductInfo_ShouldAllowNullImageUrl()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            ImageUrl = "https://example.com/original.jpg",
            IsActive = false
        };

        var command = new UpdateProductInfoCommand(
            "Updated Name",
            "Updated Description",
            null
        );

        // Act
        var storageAction = UpdateProductInfoEndpoint.UpdateProductInfo(product, command);

        // Assert - Updated properties
        product.Name.Should().Be("Updated Name");
        product.Description.Should().Be("Updated Description");
        product.ImageUrl.Should().BeNull();

        // Assert - Preserved property
        product.IsActive.Should().BeFalse();
    }
}
