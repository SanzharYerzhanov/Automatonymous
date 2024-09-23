using MassTransit;

namespace WebApplication5.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public Request<OrderState, ProcessOrder, OrderProcessed> ProcessOrder { get; private set; }
        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public State Submitted { get; private set; }
        public State Processed { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => SubmitOrder, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Request(() => ProcessOrder, x => x.ProcessOrderRequestId);
            Initially(
                When(SubmitOrder)
                    .Request(ProcessOrder, cxt => cxt.Init<ProcessOrder>(new ProcessOrder
                    {
                        OrderId = cxt.Saga.CorrelationId
                    }))
                    .TransitionTo(Submitted)
            );
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