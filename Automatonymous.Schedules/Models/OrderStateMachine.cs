using MassTransit;

namespace Automatonymous.Schedules.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public Schedule<OrderState, OrderCompletionTimeoutExpired> OrderCompletionTimeout { get; } = null!;
    public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;
    public Event<OrderCompleted> OrderCompleted { get; private set; } = null!;

    public State Submitted { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State TimedOut { get; private set; } = null!;
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => SubmitOrder, x => x.CorrelateById(cxt => cxt.Message.OrderId));
        Event(() => OrderCompleted, x => x.CorrelateById(cxt => cxt.Message.OrderId));
        Schedule(() => OrderCompletionTimeout, instance => instance.OrderCompletionTimeoutTokenId,s =>
        {
            s.Delay = TimeSpan.FromMinutes(2);
            s.Received = r => r.CorrelateById(cxt =>
                cxt.Message.OrderId);
        });  
        
        Initially(
            When(SubmitOrder)
                .Schedule(OrderCompletionTimeout, cxt => new OrderCompletionTimeoutExpired
                {
                    OrderId = cxt.Saga.CorrelationId
                })
                .TransitionTo(Submitted)
        );
        During(Submitted,
            When(OrderCompleted)
                .Unschedule(OrderCompletionTimeout)
                .TransitionTo(Completed)
        );
        // When the timeout occurs (i.e., order not completed in time)
        During(Submitted,
            When(OrderCompletionTimeout.Received)
                .Then(cxt =>
                {
                     Console.WriteLine($"Order {cxt.Saga.CorrelationId} timed out");
                })
                .TransitionTo(TimedOut)
        );
    } 
}