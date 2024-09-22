using MassTransit;

namespace WebApplication5.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; set; }
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => SubmitOrder, x =>
        {
            x.CorrelateById(context => context.Message.OrderId);
        });
        Initially(
            When(SubmitOrder)
                .Then(cxt =>
                {
                    cxt.Saga.OrderDate = DateTime.UtcNow;
                })
                .TransitionTo(Submitted));
    }
    public Event<SubmitOrder> SubmitOrder { get; set; }
}