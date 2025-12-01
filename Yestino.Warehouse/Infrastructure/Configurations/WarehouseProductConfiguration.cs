using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yestino.Warehouse.Entities;

namespace Yestino.Warehouse.Infrastructure.Configurations;

public class WarehouseProductConfiguration : IEntityTypeConfiguration<WarehouseProduct>
{
    public void Configure(EntityTypeBuilder<WarehouseProduct> builder)
    {
        builder.ToTable("WarehouseProducts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCatalogId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ReservedQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastStockUpdate)
            .IsRequired(false);

        builder.HasIndex(x => x.ProductCatalogId)
            .IsUnique()
            .HasDatabaseName("IX_WarehouseProducts_ProductCatalogId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_WarehouseProducts_IsActive");
    }
}