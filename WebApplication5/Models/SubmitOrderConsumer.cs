using MassTransit;
using WebApplication5.Models;

namespace WebApplication5.Models;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
    }
}