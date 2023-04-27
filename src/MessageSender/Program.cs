using AhoyMessageSender;
using AhoyShared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqs(builder.Environment.IsDevelopment(), builder.Configuration);

builder.Services.AddHttpClient("AhoyWhatsAppConnector", httpClient =>
{
    var whatsAppConnectorUrl = builder.Configuration.GetSection("AhoyWhatsAppConnector")["Url"];
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    httpClient.BaseAddress = new Uri(whatsAppConnectorUrl!);
});

builder.Services.AddHostedService<Consumer>();

var app = builder.Build();

app.Run();