using Broker.CommonConfig;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel((c, o) =>
{
    o.ListenAnyIP(int.Parse(c.Configuration["Http1Port"]!), lo => lo.Protocols = HttpProtocols.Http1);
});

#region Configure services

builder.Services.AddMetricServer(o => o.Port = ushort.Parse(builder.Configuration["MetricsPort"]));
BrokersRegistrar.RegisterAll(builder.Configuration.GetRequiredSection("Brokers"), builder.Services);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Configure middlewares

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpMetrics();
app.MapControllers();

#endregion

app.Run();