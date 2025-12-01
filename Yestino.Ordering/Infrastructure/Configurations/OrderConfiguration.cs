using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yestino.Ordering.Domain;

namespace Yestino.Ordering.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.CreateAt)
            .IsRequired();

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.ShippedAt)
            .IsRequired(false);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(x => x.TrackingNumber)
            .HasDatabaseName("IX_Orders_TrackingNumber");
    }
}