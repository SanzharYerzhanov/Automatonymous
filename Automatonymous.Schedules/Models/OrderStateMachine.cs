using MassTransit;

namespace Automatonymous.Schedules.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        
    }
}