# Automatonymous
Saga State Machine is a powerful tool to keep tracking the current state of messages.

# 1)  Requests

```csharp
   .Request(ProcessOrder, cxt => cxt.Init<ProcessOrder>(new ProcessOrder
   {
   OrderId = cxt.Saga.CorrelationId // Ensure CorrelationId is passed
   }))
   ```

This code sends the request **ProcessOrder**, and expects response of type **OrderProcessed**. Init<T> method is needed in order to send the message in envelope format. From what GPT produced:
When using Init, it wraps the message into a standardized envelope that may contain:
*  Headers: Metadata about the message (e.g., Content-Type, MessageId, CorrelationId).
*  Content-Type: Specifies how the message content is serialized (e.g., application/json).
*  CorrelationId: Helps in linking the message to a particular sagainstance or correlating it with other messages in a workflow.
*  ConversationId: Tracks the message across multiple components.
*  Additional Metadata: Other properties such as SourceAddress, DestinationAddress, and RequestId.

Also could send the event in following format:

```csharp
.Request(ProcessOrder, cxt => new ProcessOrder
{
OrderId = cxt.Saga.CorrelationId // Ensure CorrelationId is passed
}).TransitionTo(Submitted));
```

> [!WARNING]
> If you construct a message directly without using Init, you are bypassing the automatic envelope creation process. This means that while the message will still be sent, you may not get the required metadata (headers, etc.), and some features of MassTransit like correlation or custom headers might not work as expected.

After the response was received, the following states are available to track its status:
* Completed: when the expected response ofs received successfully
* Completed2: This event represents a second type of response to the request, if defined. Not every request will have multiple possible successful responses, but for some workflows, there may be multiple possible outcomes.
* Faulted:  This event is triggered when the request fails.
* TimeoutExpired: The timeout event


# 2) Schedules

Schedules require slightly different configuration. In the OrderStateMachine model below the OrderCompletionTimeoutExpired parameter stands for event that gets raised when the specified delays expires.

```csharp
public Schedule<OrderState, OrderCompletionTimeoutExpired> OrderCompletionTimeout { get; } = null!;
public OrderStateMachine()
{
    Schedule(() => OrderCompletionTimeout, instance => instance.OrderCompletionTimeoutTokenId,s =>
        {
            s.Delay = TimeSpan.FromMinutes(2);
            s.Received = r => r.CorrelateById(cxt =>
                cxt.Message.OrderId);
        });
}
```
> [!NOTE]  
> **Received** event in the configuration method is needed in order to handle the case when the timeout is passed

OrderCompletionTimeoutTokenId serves as an id to find and unschedule the event is necessary. Whenever Unschedule() method is called the column with the specified OrderCompletionTimeoutTokenId is removed.
In our case event scheduling is applied when the **SubmitOrder** event is consumed. If we are currently at Submitted state and the event OrderCompleted is consumed, 
the OrderCompletionTimeout event is unscheduled and the columns relevant for scheduling is removed.

In Program.cs, the following method is required in order to use delayed exchanges, it depends on RabbitMQ having the x-delayed-message exchange type, which comes from delayed messsage plugin.
```csharp
cfg.UseDelayedMessageScheduler();
```
By default, RabbitMQ image from docker does not natively support the _rabbitmq_delayed_message_exchange_ plugin, so we need to install it explicitly

1) Dockerfile in the project pulls the image from docker and customizes it by downloading the plugin 

```dockerfile
RUN curl -L https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v3.8.0/rabbitmq_delayed_message_exchange-3.8.0.ez \
  -o $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.8.0.ez
```
2) Now you can run the image without any problem
```bash
docker build -t myrabbitmq:delayed .
```

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=your_name -e RABBITMQ_DEFAULT_PASS=your_password myrabbitmq:delayed
```

## Links
Link to official documentation: https://masstransit.io/documentation/patterns/saga/state-machine





