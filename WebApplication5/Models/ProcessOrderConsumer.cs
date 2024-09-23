using MassTransit;

namespace WebApplication5.Models;

public class ProcessOrderConsumer : IConsumer<ProcessOrder>
{
    private readonly ILogger<ProcessOrderConsumer> _logger;
    public ProcessOrderConsumer(ILogger<ProcessOrderConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ProcessOrder> context)
    {
        var id = Guid.NewGuid();
        _logger.LogInformation("The processing with id {id} is generated", id);
        await context.RespondAsync(new OrderProcessed()
        {
            OrderId = context.Message.OrderId,
            ProcessingId = id
        });
    }
}