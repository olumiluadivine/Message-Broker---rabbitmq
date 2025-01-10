using MassTransit;
using RabbitMQ.Client;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-topic-exchange"));
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = ExchangeType.Topic);
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/order", async (IBus bus, OrderRequest orderRequest) =>
{
    var orderPlacedMessage = new OrderPlaced(orderRequest.orderId, orderRequest.quantity);

    await bus.Publish(orderPlacedMessage, context =>
    {
        context.SetRoutingKey(orderRequest.quantity > 10 ? "order.shipping" : "order.regular.tracking");
    });

    return Results.Created($"/order/{orderRequest.orderId}", orderPlacedMessage);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public record OrderRequest(Guid orderId, int quantity);