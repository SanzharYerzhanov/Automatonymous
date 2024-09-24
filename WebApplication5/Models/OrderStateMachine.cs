using MassTransit;

namespace WebApplication5.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public Request<OrderState, ProcessOrder, OrderProcessed> ProcessOrder { get; private set; } = null!;
        public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;
        public State Submitted { get; private set; } = null!;
        public State Processed { get; private set; } = null!;
 
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => SubmitOrder, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Request(() => ProcessOrder, x => x.ProcessOrderRequestId);
            Initially(
                When(SubmitOrder)
                    .Request(ProcessOrder, cxt => new ProcessOrder
                {
                    OrderId = cxt.Saga.CorrelationId // Ensure CorrelationId is passed
                }).TransitionTo(Submitted));
            During(Submitted,
                When(ProcessOrder.Completed)
                    .Then(cxt =>
                    {
                        cxt.Saga.ProcessingId = cxt.Message.ProcessingId;
                        cxt.Saga.ProcessOrderRequestId = Guid.NewGuid();
                    })
                    .TransitionTo(Processed));
        }
    }
}