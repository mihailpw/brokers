using Broker.Face.Grpc;
using Google.Protobuf;
using Grpc.Net.Client;
using NBomber.CSharp;

var httpClient = new HttpClient();
var httpScenario = Scenario.Create(
        "http_scenario", async _ =>
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "http://localhost:9000/event?broker=console-1")
            {
                Content = new StringContent("Test Hello")
            };
            request.Headers.Add("Accept", "text/html");

            var response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail();
        })
    .WithLoadSimulations(
        Simulation.Inject(rate: 1000,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(1)));

var grpcChannel = GrpcChannel.ForAddress("http://localhost:9100");
await grpcChannel.ConnectAsync();
var grpcClient = new Event.EventClient(grpcChannel);
var grpcScenario = Scenario.Create(
        "grpc_scenario", async _ =>
        {
            var response = await grpcClient.SendAsync(new EventRequest
            {
                Broker = "console-1",
                Payload = ByteString.CopyFromUtf8("Test Hello")
            });
            
            return response != null
                ? Response.Ok()
                : Response.Fail();
        })
    .WithLoadSimulations(
        Simulation.Inject(rate: 1000,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(1)));

NBomberRunner
    .RegisterScenarios(httpScenario, grpcScenario)
    .Run();