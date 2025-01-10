using MassTransit;
using RabbitMQ.Client;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-headers-exchange"));
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = ExchangeType.Headers);
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

    var headers = new Dictionary<string, object>();

    if (orderPlacedMessage.Quantity > 10)
    {
        headers["department"] = "shipping";
        headers["priority"] = "high";
    }
    else
    {
        headers["department"] = "tracking";
        headers["priority"] = "low";
    }

    await bus.Publish(orderPlacedMessage, context =>
    {
        context.Headers.Set("department", headers["department"]);
        context.Headers.Set("priority", headers["priority"]);
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