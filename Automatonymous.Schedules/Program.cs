using Automatonymous.Schedules.Models;
using Hangfire;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();
    x.AddConsumer<OrderCompletedConsumer>();
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
        cfg.ReceiveEndpoint("order-queue", conf =>
        {
            conf.ConfigureConsumer<SubmitOrderConsumer>(context);
            conf.ConfigureSaga<OrderState>(context);
        });
        cfg.UseDelayedMessageScheduler();
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/send-order", async (IPublishEndpoint endpoint) =>
{
    var id = Guid.NewGuid();
    Console.WriteLine($"The id {id} is generated");
    await endpoint.Publish(new SubmitOrder() { OrderId = id });
});
app.Run();
