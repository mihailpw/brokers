using Broker.CommonConfig;
using Broker.Face.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel((c, o) =>
{
    o.ListenAnyIP(int.Parse(c.Configuration["Http2Port"]!), lo => lo.Protocols = HttpProtocols.Http2);
});

#region Configure services

builder.Services.AddMetricServer(o => o.Port = ushort.Parse(builder.Configuration["MetricsPort"]));
BrokersRegistrar.RegisterAll(builder.Configuration.GetRequiredSection("Brokers"), builder.Services);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

#endregion

var app = builder.Build();

#region Configure middlewares

app.MapGrpcService<EventService>();
app.MapGrpcReflectionService();

#endregion

app.Run();