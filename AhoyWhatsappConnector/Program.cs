using AhoyWhatsappConnector.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI(config => config.DefaultModelsExpandDepth(-1));

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapSendMessageEndpoints();

app.MapWebhookEndpoints();

app.Run();