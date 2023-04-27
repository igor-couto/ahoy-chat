using AhoyShared.Configuration;
using AhoyWhatsappConnector.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

var applicationInfo = builder.Configuration.GetSection("Application").Get<ApplicationInfo>();
builder.Services.AddSwagger(applicationInfo);

builder.Services.AddSqs(builder.Environment.IsDevelopment(), builder.Configuration);
builder.Services.AddHealthChecks();

var version = builder.Configuration.GetSection("WhatsappCloudApi")["Version"];
var uri = $"https://graph.facebook.com/{version}/";

builder.Services.AddHttpClient("WhatsappCloudApiWithUserToken", httpClient =>
{
    var userToken = builder.Configuration.GetSection("WhatsappCloudApi")["UserToken"];
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {userToken}");
    httpClient.BaseAddress = new Uri(uri);
});

builder.Services.AddHttpClient("WhatsappCloudApiWithAppToken", httpClient =>
{
    var appToken = builder.Configuration.GetSection("WhatsappCloudApi")["AppToken"];
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {appToken}");
    httpClient.BaseAddress = new Uri(uri);
});

var app = builder.Build();

app.UseSwaggerConfiguration(applicationInfo);

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapSendMessageEndpoints();

app.MapWebhookEndpoints();

app.Run();