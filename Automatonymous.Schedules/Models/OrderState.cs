using MassTransit;

namespace Automatonymous.Schedules.Models;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
}