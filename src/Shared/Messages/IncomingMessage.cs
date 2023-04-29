using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public class IncomingMessage : ChatMessage {
    
    [property: JsonPropertyName("userId")]
    public Guid UserId { get; set; }
     
    [property: JsonPropertyName("customer")]
    public Customer Customer { get; init; }
}