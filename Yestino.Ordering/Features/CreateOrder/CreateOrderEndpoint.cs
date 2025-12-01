using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Features.Commands.CreateOrder;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.CreateOrder;

public static class CreateOrderEndpoint
{
    [WolverinePost("/orders")]
    public static async Task<(IResult, IStorageAction<Order>)> CreateOrder(
        CreateOrderCommand command,
        OrderingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (command.Items == null || !command.Items.Any())
        {
            return (Results.BadRequest("Order must contain at least one item"), 
                   Storage.Nothing<Order>());
        }

        if (string.IsNullOrWhiteSpace(command.CustomerAddress))
        {
            return (Results.BadRequest("Customer address is required"), 
                   Storage.Nothing<Order>());
        }

        var productIds = command.Items.Select(i => i.ProductId).ToList();
        
        var products = await dbContext.ProductReadModels
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var errors = new List<string>();
        var orderItems = new List<CreateOrderItemModel>();

        foreach (var item in command.Items)
        {
            if (item.Quantity <= 0)
            {
                errors.Add($"Quantity for product {item.ProductId} must be positive");
                continue;
            }

            if (!products.TryGetValue(item.ProductId, out var product))
            {
                errors.Add($"Product {item.ProductId} not found");
                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                errors.Add($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                continue;
            }

            orderItems.Add(new CreateOrderItemModel(
                ProductId: item.ProductId,
                Quantity: item.Quantity,
                Price: product.Price,
                ProductName: product.Name
            ));
        }

        if (errors.Any())
        {
            return (Results.BadRequest(new { Errors = errors }), 
                   Storage.Nothing<Order>());
        }

        var order = Order.Create(command.CustomerAddress, orderItems);
        
        return (Results.Created($"/orders/{order.Id}", new { OrderId = order.Id }), 
               Storage.Insert(order));
    }
}