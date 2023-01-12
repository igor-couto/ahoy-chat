namespace AhoyWhatsappConnector.Requests.Incoming;

using System.Text.Json.Serialization;

public class WebhookRequest
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("entry")]
    public List<Entry> Entry { get; set; }
}

public class Change
{
    [JsonPropertyName("field")]
    public string Field { get; set; }

    [JsonPropertyName("value")]
    public Value Value { get; set; }
}

public class Contact
{
    [JsonPropertyName("profile")]
    public Profile Profile { get; set; }

    [JsonPropertyName("wa_id")]
    public string WaId { get; set; }
}

public class Entry
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("changes")]
    public List<Change> Changes { get; set; }
}

public class Message
{
    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public Text Text { get; set; }

    [JsonPropertyName("image")]
    public Image Image { get; set; }
}

public class Metadata
{
    [JsonPropertyName("display_phone_number")]
    public string DisplayPhoneNumber { get; set; }

    [JsonPropertyName("phone_number_id")]
    public string PhoneNumberId { get; set; }
}

public class Profile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Text
{
    [JsonPropertyName("body")]
    public string Body { get; set; }
}

public class Image
{
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }
    
    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class Value
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("contacts")]
    public List<Contact> Contacts { get; set; }

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }
}

