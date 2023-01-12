using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AhoyWhatsappConnector.Requests.Outgoing;
using AhoyWhatsappConnector.Requests.Incoming;

namespace AhoyWhatsappConnector.Endpoints;

public static class Webhook
{
    public static void MapWebhookEndpoints(this WebApplication app)
    {
        app.MapGet("/webhook", VerifyWebhook);
        app.MapPost("/webhook", ReceiveMessage);
        app.MapPost("/webhook/register", RegisterWebhook);
    }

    public static IResult VerifyWebhook(WebhookVerificationQueryString webhookVerificationQueryString, [FromServices] IConfiguration configuration)
    {
        var verifyToken = configuration["VerifyToken"];

        if (webhookVerificationQueryString.Mode == "subscribe" && webhookVerificationQueryString.VerifyToken == verifyToken)
        {
            Console.WriteLine("Webhook verified with success");
            return Results.Ok(int.Parse(webhookVerificationQueryString.Challenge));
        }

        return Results.Forbid();
    }

    public static async Task<IResult> ReceiveMessage([FromServices] IHttpClientFactory httpClientFactory, WebhookRequest webhookRequest)
    {
        Console.WriteLine("Message Received: " + JsonSerializer.Serialize(webhookRequest, new JsonSerializerOptions { WriteIndented = true }));

        if(webhookRequest.Entry.FirstOrDefault().Changes.FirstOrDefault().Value.Messages.FirstOrDefault().Type == "image")
        {
            var httpClient = httpClientFactory.CreateClient("WhatsappCloudApiWithUserToken");
            var mediaId = webhookRequest.Entry.FirstOrDefault().Changes.FirstOrDefault().Value.Messages.FirstOrDefault().Image.Id;

            var response = JsonSerializer.Deserialize<GetMediaUrlResponse>((await httpClient.GetAsync($"{mediaId}")).Content.ReadAsStream());
            
            var file = (await httpClient.GetAsync(response.Url)).Content.ReadAsStringAsync();
            return Results.Ok(file);
        }
        return Results.Ok();
    }

    private static async Task<IResult> RegisterWebhook(
        string endpoint,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IConfiguration configuration)
    {
        var verifyToken = configuration["VerifyToken"];
        var content = new RegisterWebhookRequest(endpoint, verifyToken).ToContent();

        var appId = configuration.GetSection("WhatsappCloudApi")["AppId"];
        var httpClient = httpClientFactory.CreateClient("WhatsappCloudApiWithAppToken");

        var response = await httpClient.PostAsync($"{appId}/subscriptions", content);

        if (response.IsSuccessStatusCode)
            return Results.Ok();

        return Results.BadRequest();
    }
}