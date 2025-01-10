using MassTransit;
using RabbitMQ.Client;
using ShippingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("shipping-order-queue", e =>
        {
            e.Consumer<OrderPlacedConsumer>();

            e.Bind("order-placed-exchange", x =>
            {
                x.RoutingKey = "order.shipping";
                x.ExchangeType = ExchangeType.Direct;
            });

            e.UseMessageRetry(r => r.Interval(2, 100));
            e.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)));
            e.UseKillSwitch(options => options
                .SetActivationThreshold(10)
                .SetTripThreshold(0.1)
                .SetRestartTimeout(s: 20));
        });
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();