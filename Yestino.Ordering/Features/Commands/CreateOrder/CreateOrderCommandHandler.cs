using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.Commands.CreateOrder;

public static class CreateOrderCommandHandler
{
    public static Guid Handle(CreateOrderCommand command, OrderingDbContext dbContext)
    {
        var order = Order.Create(
            command.CustomerAddress,
            command.Items
                .Select(i => new CreateOrderItemModel(i.ProductId, i.Quantity, 123, "todo"))
                .ToList()
        );

        dbContext.Orders.Add(order);

        return order.Id;
    }
}