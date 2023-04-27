namespace AhoyWhatsappConnector.Requests.Incoming;

using System.Text.Json.Serialization;

public class GetMediaUrlResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; }

    [JsonPropertyName("file_size")]
    public int FileSize { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; }
}