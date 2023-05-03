using System.Text.Json.Serialization;

namespace AhoyContracts.Messages.WhatsApp;

public class TextMessage
{
    [property: JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; init; }
    
    [property: JsonPropertyName("message")]
    public string Message { get; init; }
}