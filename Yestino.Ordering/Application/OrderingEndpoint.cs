using Carter;

namespace Yestino.Ordering.Application;

public abstract class OrderingEndpoint : CarterModule
{
    public OrderingEndpoint() : base("ordering")
    {
        WithTags("Ordering");
    }
}