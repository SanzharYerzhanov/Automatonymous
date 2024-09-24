using MassTransit;

namespace Automatonymous.Schedules.Models;

public class OrderCompletedConsumer : IConsumer<OrderCompleted>
{
    public async Task Consume(ConsumeContext<OrderCompleted> context)
    {
        
    }
}