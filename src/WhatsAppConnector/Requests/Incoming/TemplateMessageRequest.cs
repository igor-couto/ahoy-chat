namespace AhoyWhatsappConnector.Requests.Incoming;

public class TemplateMessageRequest
{
    public string PhoneNumber { get; set; }

    public string TemplateMessageName { get; set; }

    public List<string> Parameters { get; set; }
}