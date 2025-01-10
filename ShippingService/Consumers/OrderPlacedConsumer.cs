using MassTransit;
using SharedMessages.Messages;

namespace ShippingService.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Console.WriteLine($"Order {context.Message.OrderId} placed with quantity {context.Message.Quantity}");
            return Task.CompletedTask;
        }
    }
}