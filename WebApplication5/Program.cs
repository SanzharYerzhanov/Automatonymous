using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
 using WebApplication5.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();
    x.AddConsumer<ProcessOrderConsumer>();
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(conf =>
        {
            conf.ConcurrencyMode = ConcurrencyMode.Optimistic;
            conf.AddDbContext<SagaDbContext, OrderStateDbContext>((provider, opts) =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("Automatonymous"));
            });
        });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseDelayedMessageScheduler();
        cfg.Host("rabbitmq://localhost", conf =>
        {
            conf.Username(builder.Configuration["UserSettings:UserName"]!);
            conf.Password(builder.Configuration["UserSettings:Password"]!);
        });
        cfg.ReceiveEndpoint("order-queue", conf =>
        {
            conf.ConfigureSaga<OrderState>(context);
            conf.ConfigureConsumer<SubmitOrderConsumer>(context);
            conf.ConfigureConsumer<ProcessOrderConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/send-order", async (IPublishEndpoint endpoint) =>
{
    await endpoint.Publish(new SubmitOrder()
    {
        OrderId = Guid.NewGuid()
    });
    
});

app.Run();