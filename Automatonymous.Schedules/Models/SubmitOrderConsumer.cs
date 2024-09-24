using MassTransit;

namespace Automatonymous.Schedules.Models;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        
    }
}