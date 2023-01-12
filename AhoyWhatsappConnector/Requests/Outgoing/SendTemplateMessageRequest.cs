namespace AhoyWhatsappConnector.Requests.Outgoing;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AhoyWhatsappConnector.Requests.Incoming;

public class SendTemplateMessageRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct => "whatsapp";

    [JsonPropertyName("type")]
    public string Type => "template";

    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonPropertyName("template")]
    public Template Template { get; set; }

    public SendTemplateMessageRequest(TemplateMessageRequest templateMessageRequest)
    {
        To = templateMessageRequest.PhoneNumber;
        Template = new() { Name = templateMessageRequest.TemplateMessageName, Language = new() };

        if (templateMessageRequest.Parameters is not null && templateMessageRequest.Parameters.Any())
        {
            Template.Components = new();

            var parameters = new List<Parameter>();

            foreach (var textParameter in templateMessageRequest.Parameters)
                parameters.Add(new Parameter() { Text = textParameter });

            Template.Components.Add(new Component() { Parameters = parameters });
        }
    }

    public StringContent ToContent()
    {
        var jsonString = JsonSerializer.Serialize(this);
        return new StringContent(jsonString, Encoding.UTF8, "application/json");
    }
}

public class Template
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("language")]
    public Language Language { get; set; }

    [JsonPropertyName("components")]
    public List<Component> Components { get; set; }
}

public class Language
{
    [JsonPropertyName("code")]
    public string Code => "pt_BR";
}

public class Component
{
    [JsonPropertyName("type")]
    public string Type => "body";

    [JsonPropertyName("parameters")]
    public List<Parameter> Parameters { get; set; }
}

public class Parameter
{
    [JsonPropertyName("type")]
    public string Type => "text";

    [JsonPropertyName("text")]
    public string Text { get; set; }
}