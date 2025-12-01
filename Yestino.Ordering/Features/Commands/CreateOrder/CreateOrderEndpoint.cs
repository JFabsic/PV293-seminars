using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;
using Yestino.Ordering.Application;

namespace Yestino.Ordering.Features.Commands.CreateOrder;

public class CreateOrderEndpoint : OrderingEndpoint
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders",
            async ([FromBody] CreateOrderCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
                await bus.InvokeAsync<Guid>(command, cancellationToken));
    }
}