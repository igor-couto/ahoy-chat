using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public class OutgoingMessage : ChatMessage {

    [property: JsonPropertyName("customerContact")]
    public string CustomerContact { get; set; }
}