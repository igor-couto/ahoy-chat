using AhoyMessageReceiver;
using AhoyShared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqs(builder.Environment.IsDevelopment(), builder.Configuration);

builder.Services.AddHttpClient("AhoyChat", httpClient =>
{
    var ahoyChatUrl = builder.Configuration.GetSection("AhoyChat")["Url"];
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    httpClient.BaseAddress = new Uri(ahoyChatUrl!);
});

builder.Services.AddHostedService<Consumer>();

var app = builder.Build();

app.Run();