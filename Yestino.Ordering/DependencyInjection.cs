using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wolverine.EntityFrameworkCore;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering;

public static class DependencyInjection
{

    public static WebApplicationBuilder AddOrderingModule(this WebApplicationBuilder builder, List<Assembly> moduleAssemblies)
    {
        moduleAssemblies.Add(typeof(OrderingDbContext).Assembly);
        
        var connectionString = builder.Configuration.GetConnectionString("YestinoDb");
        
        builder.Services.AddDbContextWithWolverineIntegration<OrderingDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }
}