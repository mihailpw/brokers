using Broker.Face.Consumers.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((c, o) =>
{
    o.ListenAnyIP(int.Parse(c.Configuration["Http1Port"]), lo => lo.Protocols = HttpProtocols.Http1);
    o.ListenAnyIP(int.Parse(c.Configuration["Http2Port"]), lo => lo.Protocols = HttpProtocols.Http2);
});

#region Configure services

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddSingleton(builder.Configuration.GetSection("Kafka").Get<KafkaConsumerService.Options>());
builder.Services.AddHostedService<KafkaConsumerService>();

#endregion

var app = builder.Build();

#region Configure middlewares

app.MapGrpcService<GrpcConsumerService>();
app.MapControllers();

#endregion

app.Run();