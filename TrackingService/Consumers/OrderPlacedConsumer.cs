using MassTransit;
using SharedMessages.Messages;

namespace TrackingService.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            var orderPlaced = context.Message;
            Console.WriteLine($"Order placed: {orderPlaced.OrderId}");
            return Task.CompletedTask;
        }
    }
}