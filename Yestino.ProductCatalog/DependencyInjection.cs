using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;
using Yestino.ProductCatalog.Infrastructure;

namespace Yestino.ProductCatalog;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddProductCatalogModule(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("YestinoDb");

        builder.Services.AddDbContextWithWolverineIntegration<ProductCatalogDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }
}