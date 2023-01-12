using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace AhoyWhatsappConnector.Requests.Incoming;

public class WebhookVerificationQueryString
{
    [FromQuery(Name = "hub.mode")]
    public string Mode { get; set; }

    [FromQuery(Name = "hub.verify_token")]
    public string VerifyToken { get; set; }

    [FromQuery(Name = "hub.challenge")]
    public string Challenge { get; set; }

    public static ValueTask<WebhookVerificationQueryString> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var result = new WebhookVerificationQueryString
        {
            Mode = context.Request.Query["hub.mode"],
            VerifyToken = context.Request.Query["hub.verify_token"],
            Challenge = context.Request.Query["hub.challenge"]
        };

        return ValueTask.FromResult<WebhookVerificationQueryString>(result);
    }
}