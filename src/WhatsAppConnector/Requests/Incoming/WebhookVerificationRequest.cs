namespace AhoyWhatsappConnector.Requests.Incoming;

using System.Text.Json.Serialization;

public class WebhookVerificationRequest
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("verify_token")]
    public string VerifyToken { get; set; }

    [JsonPropertyName("challenge")]
    public string Challenge { get; set; }
}