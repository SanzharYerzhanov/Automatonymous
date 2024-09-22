using MassTransit;
using WebApplication5.Models;

namespace WebApplication5.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
         
    }
}