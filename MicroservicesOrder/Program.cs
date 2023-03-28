using MassTransit;
using MicroservicesOrder;
using SharedMassege;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddMassTransit(x =>
//{
// x.AddConsumer<MyMessageConsumer>();

//x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
//{
//    cfg.Host("rabbitmq://localhost", h =>
//    {
//        h.Username("guest");
//        h.Password("123456");
//    });

//    cfg.ReceiveEndpoint("Queue1", ep =>
//    {
//        ep.PrefetchCount = 16;
//        ep.UseMessageRetry(r => r.Interval(2, 100));
//        ep.ConfigureConsumer<MyMessageConsumer>(provider);
//    });
//}));
//});
var rabbitMqSettings = builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
//builder.Services.AddMassTransit(mt => mt.AddMassTransit(x => {
//    x.UsingRabbitMq((cntxt, cfg) => {
//        cfg.Host(rabbitMqSettings.Uri, c => {
//            c.Username(rabbitMqSettings.UserName);
//            c.Password(rabbitMqSettings.Password);
//        });
//        //cfg.ReceiveEndpoint("samplequeue");

//    });
//}));
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", h =>
    {
        h.Username(rabbitMqSettings.UserName);
        h.Password(rabbitMqSettings.Password);
    });
    cfg.ReceiveEndpoint("order-service", e =>
    {
        e.Lazy = true;
        e.Consumer<OrderConsumer>();
    });
});

var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await busControl.StartAsync(cancellation.Token);
try
{
    Console.WriteLine("Bus started");
    var endpoint = await busControl.GetSendEndpoint(new Uri("exchange: order"));
    await busControl.Publish<UpdateOrder>( new
          { Type = "1235"});
    await Task.Run(Console.ReadLine);
}
finally
{
    await busControl.StopAsync(CancellationToken.None);
}

//builder.Services.AddMassTransit(x => x.UsingRabbitMq());
//builder.Services.AddMassTransitHostedService();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
