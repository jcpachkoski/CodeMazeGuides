using MassTransit;
using Newtonsoft.Json;
using SharedModels;

// We configure our Consumer to use MassTransit.
// We create an IBusControl, using the static Bus class provided by MassTransit,
// which we’re going to configure to use RabbitMQ. We must then configure the ReceiveEndpoint,
// which will receive messages from the order-created-event queue. Finally, we use our previously
// created OrderCreatedConsumer to consume messages from this queue.
// With our Consumer configured to receive messages, the final thing we need to do is start our bus.
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("order-created-event", e =>
    {
        e.Consumer<OrderCreatedConsumer>();
    });

});

await busControl.StartAsync(new CancellationToken());

try
{
    Console.WriteLine("Press enter to exit");
    await Task.Run(() => Console.ReadLine());
}
finally
{
    await busControl.StopAsync();
}

// First, we create a class called OrderCreatedConsumer, ensuring we implement the IConsumer generic interface provided by MassTransit,
// using our OrderCreated interface defined in our ShareModels library. In the Consume method, we can simply serialize the message object
// and log the message to the console for the purposes of this article.
class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var jsonMessage = JsonConvert.SerializeObject(context.Message);
        Console.WriteLine($"OrderCreated message: {jsonMessage}");
    }

    /*
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        // var jsonMessage = JsonConvert.SerializeObject(context.Message);
        // Console.WriteLine($"OrderCreated message: {jsonMessage}");

        Console.WriteLine(context.Message);
        return Task.CompletedTask;
    }
    */
}
