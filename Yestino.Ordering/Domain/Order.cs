using Yestino.Common.Domain;
using Yestino.OrderingContracts.DomainEvents;

namespace Yestino.Ordering.Domain;

public class Order : AggregateRoot
{
    public DateTimeOffset CreateAt { get; private set; }
    public string CustomerAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? TrackingNumber { get; private set; }
    public DateTimeOffset? ShippedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items;

    public static Order Create(string customerAddress, ICollection<CreateOrderItemModel> items)
    {
        var order = new Order
        {
            CustomerAddress = customerAddress,
            CreateAt = DateTimeOffset.Now,
            Status = OrderStatus.Created,
        };

        order._items.AddRange(items.Select(i => new OrderItem
        {
            OrderId = order.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity,
        }));

        order.RaiseDomainEvent(
            new OrderCreated(
                AggregateId: order.Id,
                Items: order.Items.Select(i => new OrderCreatedItem(
                    ProductId: i.ProductId,
                    ProductName: i.ProductName,
                    Price: i.Price,
                    Quantity: i.Quantity
                )).ToList())
        );

        return order;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Order is already cancelled");
        }

        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException($"Cannot cancel order with status {Status}");
        }

        Status = OrderStatus.Cancelled;

        RaiseDomainEvent(
            new OrderCancelled(
                AggregateId: Id,
                Items: Items.Select(i => new OrderCancelledItem(
                    ProductId: i.ProductId,
                    ProductName: i.ProductName,
                    Quantity: i.Quantity,
                    Price: i.Price
                )).ToList())
        );
    }

    public void Ship(string trackingNumber)
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot ship cancelled order");
        }

        if (Status == OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Order is already shipped");
        }

        if (Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Order is already delivered");
        }

        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            throw new ArgumentException("Tracking number is required", nameof(trackingNumber));
        }

        Status = OrderStatus.Shipped;
        TrackingNumber = trackingNumber;
        ShippedAt = DateTimeOffset.Now;

        RaiseDomainEvent(
            new OrderShipped(
                AggregateId: Id,
                Items: Items.Select(i => new OrderShippedItem(
                    ProductId: i.ProductId,
                    ProductName: i.ProductName,
                    Quantity: i.Quantity,
                    Price: i.Price
                )).ToList(),
                TrackingNumber: trackingNumber,
                ShippedAt: ShippedAt.Value
            )
        );
    }
}

public enum OrderStatus
{
    Created,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public record CreateOrderItemModel(Guid ProductId, int Quantity, decimal Price, string ProductName);