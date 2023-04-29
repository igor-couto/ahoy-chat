using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public record Content(
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("text")]
    string Text
);