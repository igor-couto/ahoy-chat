using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();

app.MapGet("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

    var data = Encoding.ASCII.GetBytes($"Ahoooy {DateTime.UtcNow}");

    await webSocket.SendAsync(
        data,
        WebSocketMessageType.Text,
        true,
        CancellationToken.None
    );
});

await app.RunAsync();