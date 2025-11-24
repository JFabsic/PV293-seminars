namespace Yestino.Ordering.Features.Commands.CreateOrder;

public record CreateOrderCommand(ICollection<CreateOrderItem> Items, string CustomerAddress);

public record CreateOrderItem(Guid ProductId, int Quantity);