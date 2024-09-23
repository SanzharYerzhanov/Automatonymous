# Automatonymous
just exploring how sagas work in masstransit. To be honest, spend 2 motherfucking days, in attempt to solve the problem almost become bald, lol. The idea is simple, the Request is sent when SubmitOrder event is consumed. 

1) `.Request(ProcessOrder, cxt => cxt.Init<ProcessOrder>(new ProcessOrder
   {
   OrderId = cxt.Saga.CorrelationId // Ensure CorrelationId is passed
   }))`

This code sends the request **ProcessOrder**, and expects response of type **OrderProcessed**. Init<T> method is needed in order to send the message in envelope format. From what GPT produced:
When using Init, it wraps the message into a standardized envelope that may contain:
*  Headers: Metadata about the message (e.g., Content-Type, MessageId, CorrelationId).
*  Content-Type: Specifies how the message content is serialized (e.g., application/json).
*  CorrelationId: Helps in linking the message to a particular sagainstance or correlating it with other messages in a workflow.
*  ConversationId: Tracks the message across multiple components.
*  Additional Metadata: Other properties such as SourceAddress, DestinationAddress, and RequestId.

Also could send the event in following format:

`.Request(ProcessOrder, cxt => new ProcessOrder
{
OrderId = cxt.Saga.CorrelationId // Ensure CorrelationId is passed
}).TransitionTo(Submitted));`

> [!WARNING]
> If you construct a message directly without using Init, you are bypassing the automatic envelope creation process. This means that while the message will still be sent, you may not get the required metadata (headers, etc.), and some features of MassTransit like correlation or custom headers might not work as expected.

After the response was received, the following states are available to track its status:
* Completed: when the expected response ofs received successfully
* Completed2: This event represents a second type of response to the request, if defined. Not every request will have multiple possible successful responses, but for some workflows, there may be multiple possible outcomes.
* Faulted:  This event is triggered when the request fails.
* TimeoutExpired: The timeout event


Now, i need to learn about schedules in Saga State Machine.
For the case, if you wonder, the [documentation](https://masstransit.io/documentation/patterns/saga/state-machine#schedule) is what causes complications when it comes to learning, so need to learn on my own.

