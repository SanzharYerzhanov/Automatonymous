using MassTransit;

namespace WebApplication5.Models;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid? ProcessOrderRequestId { get; set; }
    public Guid? ProcessingId { get; set; }
}