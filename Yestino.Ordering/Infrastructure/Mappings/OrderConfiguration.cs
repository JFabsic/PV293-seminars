using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yestino.Ordering.Domain;

namespace Yestino.Ordering.Infrastructure.Mappings;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.CreateAt).IsRequired();
        
        builder.OwnsMany(x => x.Items, orderItem =>
        {
            orderItem.HasKey(x => x.Id);
            orderItem.Property(x => x.Id).IsRequired();
            orderItem.Property(x => x.ProductId).IsRequired();
            orderItem.Property(x => x.Quantity).IsRequired();
            orderItem.Property(x => x.Price).IsRequired();
            
            orderItem.WithOwner().HasForeignKey(x => x.OrderId);
        });
    }
}