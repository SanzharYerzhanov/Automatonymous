using MassTransit;

namespace Automatonymous.Schedules.Models;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        throw new NotImplementedException();
    }
}