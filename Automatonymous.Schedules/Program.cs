using Automatonymous.Schedules.Models;
using Hangfire;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(x =>
{
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("Hangfire"));
});
builder.Services.AddHangfireServer();
builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(e =>
        {
            e.ConcurrencyMode = ConcurrencyMode.Optimistic;
            e.AddDbContext<SagaDbContext, OrderStateDbContext>((provider, opts) =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("Automatonymous"));
            });
        });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username(builder.Configuration["UserSettings:UserName"]!);
            h.Password(builder.Configuration["UserSettings:Password"]!);
        });
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();
app.UseHangfireDashboard(); //accessible via /hangfire endpoint
app.MapGet("/", () => "Hello World!");
app.Run();