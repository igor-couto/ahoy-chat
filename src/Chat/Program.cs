using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using AhoyShared.Configuration;
using AhoyChat;
using Amazon.SQS;
using AhoyContracts.Messages;
using System.Text.Json;
using AhoyChat.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using AhoyChat.Entities;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

var applicationInfo = builder.Configuration.GetSection("Application").Get<ApplicationInfo>();
builder.Services.AddSwagger(applicationInfo);
builder.Services.AddCorsConfiguration();

builder.Services.AddSingleton<ChatRecordRepository>();

builder.Services.AddSqs(isDevelopment, builder.Configuration);

var app = builder.Build();
app.UseSwaggerConfiguration(applicationInfo);
app.UseRouting();
app.UseCorsConfiguration();
app.UseWebSockets();

var connectedUsers = new ConcurrentDictionary<Guid, WebSocket>();

var serviceProvider = builder.Services.BuildServiceProvider();
var sqsClient = serviceProvider.GetRequiredService<IAmazonSQS>();
var publisher = new Publisher(sqsClient, builder.Configuration["AWS:SQS:QueueName"]!);

app.MapGet("/ws/{userId:guid}", async (Guid userId, HttpContext context, [FromServices] ChatRecordRepository chatRecordRepository) =>
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

            var outgoingChatMessage = JsonSerializer.Deserialize<OutgoingMessage>(receivedText);

            // TODO: put the publish and the save actions inside a transaction
            await publisher.Publish(outgoingChatMessage);

            await chatRecordRepository.AddNewOutgoingMessage(new ChatRecord{
                UserId = userId.ToString(),
                Customer = new Customer(string.Empty, outgoingChatMessage.CustomerContact, string.Empty),
                MessageHistory = new List<Message>{ new Message(outgoingChatMessage.Id, outgoingChatMessage.Date, "sent", new Content(outgoingChatMessage.Content.Type, outgoingChatMessage.Content.Text) )} 
            });
        }
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    Console.WriteLine($"Connection Closed: {userId}.");
    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

    connectedUsers.TryRemove(userId, out _);
});

app.MapPost("/message/{userId:guid}", async (Guid userId, IncomingMessage incomingChatMessage, [FromServices] ChatRecordRepository chatRecordRepository) => {

    if (connectedUsers.TryGetValue(userId, out var client))
    {
        Console.WriteLine($"Sending message from client {userId} to user: {JsonSerializer.Serialize(incomingChatMessage, new JsonSerializerOptions { WriteIndented = true })}");

        var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(incomingChatMessage));

        await client.SendAsync(
            data,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );

        await chatRecordRepository.AddNewIncomingMessage(new ChatRecord{
            UserId = userId.ToString(),
            Customer = new Customer(incomingChatMessage.Customer.Name, incomingChatMessage.Customer.Contact, incomingChatMessage.Customer.ProfilePicUrl),
            MessageHistory = new List<Message>{ new Message(incomingChatMessage.Id, incomingChatMessage.Date, "incoming", new Content(incomingChatMessage.Content.Type, incomingChatMessage.Content.Text) )} 
        });

        return Results.Ok();
    }
    else
    {
        Console.WriteLine($"ERROR: user Id {userId} is not found.");
        return Results.BadRequest();
    }
});

app.MapGet("/messages/{userId:guid}", async (Guid userId, HttpContext context, [FromServices] ChatRecordRepository chatRecordRepository) =>
    await chatRecordRepository.GetMessagesFromUser(userId)
);

await app.RunAsync();