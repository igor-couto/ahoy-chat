using AhoyWhatsappConnector.Requests.Incoming;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AhoyWhatsappConnector.Requests.Outgoing;

public class SendTextMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct => "whatsapp";

    [JsonPropertyName("type")]
    public string Type => "text";

    [JsonPropertyName("recipient_type")]
    public string RecipientType => "individual";

    [JsonPropertyName("to")]
    public string To { get; private set; }

    [JsonPropertyName("text")]
    public Text Text { get; private set; }

    public SendTextMessageRequest(string whatsappId, string text)
    {
        To = whatsappId;
        Text = new() { Body = text };
    }

    public StringContent ToContent()
    {
        var jsonString = JsonSerializer.Serialize(this);
        return new StringContent(jsonString, Encoding.UTF8, "application/json");
    }
}