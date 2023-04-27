using System.Text.Json.Serialization;

namespace AhoyContracts.Messages;

public record ChatMessage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("date")] DateTime Date,
    [property: JsonPropertyName("from")] From From,
    [property: JsonPropertyName("to")] To To,
    [property: JsonPropertyName("content")] Content Content
);

public record Content(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] string Text
    // [property: JsonPropertyName("url")] string Url,
    // [property: JsonPropertyName("repliedMessageId")] string RepliedMessageId
);

public record From(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("contact")] string Contact,
    [property: JsonPropertyName("role")] string Role
);

public record To(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("contact")] string Contact,
    [property: JsonPropertyName("role")] string Role
);