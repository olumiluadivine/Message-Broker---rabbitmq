using MassTransit;
using RabbitMQ.Client;
using TrackingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("tracking-order-placed", e =>
        {
            e.Consumer<OrderPlacedConsumer>();

            e.Bind("order-headers-exchange", x =>
            {
                x.ExchangeType = ExchangeType.Headers;
                x.SetBindingArgument("department", "tracking");
                x.SetBindingArgument("priority", "low");
                x.SetBindingArgument("x-match", "all");
            });
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