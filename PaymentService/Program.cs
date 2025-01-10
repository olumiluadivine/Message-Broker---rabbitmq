using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("inventory-reserved", e =>
        {
            e.Consumer<InventoryPlacedConsumer>();
        });
    });
});

var app = builder.Build();
app.Run();

public class InventoryPlacedConsumer : IConsumer<InventoryReserved>
{
    public async Task Consume(ConsumeContext<InventoryReserved> context)
    {
        Console.WriteLine($"Inventory reserved for Order {context.Message.OrderId}");
        await context.Publish(new PaymentCompleted(context.Message.OrderId));
    }
}