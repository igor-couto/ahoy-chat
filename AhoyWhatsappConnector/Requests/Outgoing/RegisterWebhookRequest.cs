namespace AhoyWhatsappConnector.Requests.Outgoing;

using System.Web;

public class RegisterWebhookRequest
{
    private readonly string _callbackUrl;
    private readonly string _verifyToken;

    public RegisterWebhookRequest(string endpoint, string verifyToken)
    {
        _callbackUrl = endpoint;
        _verifyToken = verifyToken;
    }

    public FormUrlEncodedContent ToContent()
    {
        var urlEncoded = new Dictionary<string, string>();

        urlEncoded.Add("object", "whatsapp_business_account");
        urlEncoded.Add("callback_url", _callbackUrl);
        urlEncoded.Add("verify_token", _verifyToken);
        urlEncoded.Add("fields", "messages");

        return new FormUrlEncodedContent(urlEncoded);
    }
}