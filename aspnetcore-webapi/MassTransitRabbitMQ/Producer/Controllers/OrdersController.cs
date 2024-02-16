using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

// Go to this article and read it:
// https://code-maze.com/masstransit-rabbitmq-aspnetcore/#:~:text=MassTransit%20is%20a%20free%2C%20open,%2Dbased%2C%20loosely%20coupled%20applications.
// With MassTransit configured in Program.cs to use RabbitMQ and our DTO defined,
// let’s now create an API controller that will publish a message,
// or more specifically an event, using our OrderCreated interface:
namespace Producer.Controllers
{
    /*
    Why Use MassTransit?
    There are a few benefits to choosing to use a library such as MassTransit, instead of working with the native message broker library. Firstly, by abstracting the underlying message broker logic, we can work with multiple message brokers, without having to completely rewrite our code. This allows us to work with something such as the InMemory transport when working locally, then when deploying our code, use another transport such as Azure Service Bus or Amazon Simple Queue Service.
    Additionally, when we work with a message-based architecture, there are a lot of specific patterns we need to be aware of and implement, such as retry, circuit breaker, outbox to name a few. MassTransit handles all of this for us, along with many other features such as exception handling, distributed transactions, and monitoring.
    Now that we have an understanding of what MassTransit is and why we would use it, let’s see how we can use it along with RabbitMQ in ASP.NET Core.

    */
    /*
    MassTransit includes the namespace for message contracts, so we can use a shared class/interface to set up our bindings correctly.
    Step 1. Message is a contract, defined code first by creating a .NET class or interface...we create the OrderCreated interface first.
    Step 2. Command is a type of message, specifically used to tell a service to do something.
    These message types are sent to an endpoint (queue) and will be expressed using a verb-noun sequence.
    Note: Events are another message type, signifying that something has happened. Events are published to
    one or multiple consumers and will be expressed using noun-verb (past tense) sequence.
    */
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        // This is an endpoint.
        // We can create an endpoint for creating an order.
        // This post method is a verb-noun which is a commmand named CreateOrder
        // Within the method, we call the generic Publish method on the
        // IPublishEndpoint interface, using our OrderCreated interface to define what type of event we are going to be publishing.
        // This is all we require to publish an event to our configured transport, RabbitMQ.
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            for (int i = 0; i < 10000; i++)
            {
                // We publish an event which is noun-verb past tense.
                // <OrderCreated>, below, is the type of event to publish.
                await _publishEndpoint.Publish<OrderCreated>(new
                {
                    // This is an anonymous type using the same property names that are defined in our OrderCreated interface.
                    // We pass in the view model Dto to publish and event named OrderCreated.
                    // Here we are building an OrderCreated.
                    Id = 1,
                    orderDto.ProductName,
                    orderDto.Quantity,
                    orderDto.Price
                });
            }

            return Ok();
        }
    }
}
