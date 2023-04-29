using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public class ChatMessage
{
    [property: JsonPropertyName("id")]
    public string Id { get; init; }
    
    [property: JsonPropertyName("date")]
    public DateTime Date { get; init; }

    [property: JsonPropertyName("content")]
    public Content Content { get; init; }
}