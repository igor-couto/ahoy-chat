using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public record Customer(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("contact")] string Contact,
    [property: JsonPropertyName("profilePicUrl")] string ProfilePicUrl
);