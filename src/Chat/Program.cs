using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using AhoyShared.Configuration;
using AhoyChat;
using Amazon.SQS;
using AhoyContracts.Messages;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

var applicationInfo = builder.Configuration.GetSection("Application").Get<ApplicationInfo>();
builder.Services.AddSwagger(applicationInfo);

builder.Services.AddSqs(isDevelopment, builder.Configuration);

var app = builder.Build();
app.UseSwaggerConfiguration(applicationInfo);
app.UseWebSockets();

var connectedUsers = new ConcurrentDictionary<Guid, WebSocket>();

var serviceProvider = builder.Services.BuildServiceProvider();
var sqsClient = serviceProvider.GetRequiredService<IAmazonSQS>();
var publisher = new Publisher(sqsClient, builder.Configuration["AWS:SQS:QueueName"]!);

app.MapGet("/ws/{userId:guid}", async (Guid userId, HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        return;
    }

    Console.WriteLine($"User Connected! Id: {userId}");

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

    connectedUsers.TryAdd(userId, webSocket);

    var buffer = new byte[1024 * 4];
    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine($"Message from user {userId}: {receivedText}");

            var chatMessage = JsonSerializer.Deserialize<ChatMessage>(receivedText);

            await publisher.Publish(chatMessage);
        }
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    Console.WriteLine($"Connection Closed: {userId}.");
    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

    connectedUsers.TryRemove(userId, out _);
});

app.MapPost("/message/{userId:guid}", async (Guid userId, ChatMessage message) => {

    if (connectedUsers.TryGetValue(userId, out var client))
    {
        Console.WriteLine($"Sending message from client {userId} to user: {message.Content.Text}");

        var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await client.SendAsync(
            data,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
        return Results.Ok();
    }
    else
    {
        Console.WriteLine($"ERROR: user Id {userId} is not found.");
        return Results.BadRequest();
    }
});

app.MapGet("/messages/{userId:guid}", async (Guid userId, HttpContext context) =>
{
    
});

await app.RunAsync();