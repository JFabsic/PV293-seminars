using JasperFx.CodeGeneration;
using JasperFx.Core;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;
using Yestino.Common;

namespace Yestino.Wolverine;

public static class WolverineSetup
{
    static readonly TimeSpan[] DefaultRetryIntervals = [50.Milliseconds(), 250.Milliseconds(), 2.Seconds()];

    public static WebApplicationBuilder SetupWolverine(this WebApplicationBuilder builder)
    {
        builder.Host.UseWolverine(opts =>
        {
            opts.ApplicationAssembly = typeof(Program).Assembly;
            opts.Discovery.IncludeAssembly(typeof(ProductCatalog.DependencyInjection).Assembly);
            // TODO: register modules here

            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
            if (builder.Environment.IsDevelopment())
            {
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;
                opts.Durability.Mode = DurabilityMode.Solo;
            }

            var connectionString = builder.Configuration.GetConnectionString("YestinoDb")
                                   ?? throw new InvalidOperationException("Connection string 'YestinoDb' not found.");
            opts.PersistMessagesWithPostgresql(connectionString, "wolverine");


            opts.Policies.UseDurableLocalQueues();
            opts.Policies.UseDurableInboxOnAllListeners();
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();


            opts.UseFluentValidation();
            

            opts.Durability.MessageStorageSchemaName = "wolverine";
            opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
            opts.Durability.MessageIdentity = MessageIdentity.IdAndDestination;
            opts.Policies.AutoApplyTransactions();

            opts.OnException<DbUpdateException>().RetryWithCooldown(DefaultRetryIntervals)
                .Then.ScheduleRetryIndefinitely(5.Minutes());
            opts.OnException<TimeoutException>().RetryWithCooldown(DefaultRetryIntervals)
                .Then.ScheduleRetryIndefinitely(5.Minutes());

            opts.OnException<DbUpdateConcurrencyException>().RetryWithCooldown(DefaultRetryIntervals)
                .Then.ScheduleRetryIndefinitely(5.Minutes());
        });

        return builder;
    }
}