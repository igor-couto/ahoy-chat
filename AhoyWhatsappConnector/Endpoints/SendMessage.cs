namespace AhoyWhatsappConnector.Endpoints;

using Microsoft.AspNetCore.Mvc;
using AhoyWhatsappConnector.Requests.Incoming;
using AhoyWhatsappConnector.Requests.Outgoing;

public static class SendMessage
{
    public static void MapSendMessageEndpoints(this WebApplication app)
    {
        app.MapPost("/message/text", SendTextMessage);
        app.MapPost("/message/template", SendTemplateMessage);
    }

    private static async Task<IResult> SendTextMessage(
        string phoneNumber,
        string message,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IConfiguration configuration)
    {
        var httpClient = httpClientFactory.CreateClient("WhatsappCloudApiWithUserToken");

        var content = new SendTextMessageRequest(phoneNumber, message).ToContent();

        var phoneNumberId = configuration.GetSection("WhatsappCloudApi")["PhoneNumberId"];

        await httpClient.PostAsync($"/{phoneNumberId}/messages", content);

        return Results.Ok();
    }

    private static async Task<IResult> SendTemplateMessage(
        TemplateMessageRequest templateMessageRequest,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IConfiguration configuration)
    {
        var httpClient = httpClientFactory.CreateClient("WhatsappCloudApiWithUserToken");

        var content = new SendTemplateMessageRequest(templateMessageRequest).ToContent();

        var phoneNumberId = configuration.GetSection("WhatsappCloudApi")["PhoneNumberId"];

        await httpClient.PostAsync($"/{phoneNumberId}/messages", content);

        return Results.Ok();
    }
}