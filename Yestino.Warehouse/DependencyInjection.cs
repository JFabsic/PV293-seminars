using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wolverine.EntityFrameworkCore;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWarehouseModule(this WebApplicationBuilder builder, List<Assembly> moduleAssemblies)
    {
        moduleAssemblies.Add(typeof(WarehouseDbContext).Assembly);
        
        var connectionString = builder.Configuration.GetConnectionString("YestinoDb");
        
        builder.Services.AddDbContextWithWolverineIntegration<WarehouseDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }
}