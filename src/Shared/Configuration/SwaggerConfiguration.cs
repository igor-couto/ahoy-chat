using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AhoyShared.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwagger(this IServiceCollection services, ApplicationInfo applicationInfo)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc($"v{applicationInfo.VersionMajor}",
            new OpenApiInfo
            {
                Title = applicationInfo.Name,
                Description = "TODO description here",
                Version = $"v{applicationInfo.VersionMajor}",
                Contact = new OpenApiContact { Name = "Igor Couto", Email = "igor.fcouto@gmail.com", Url = new Uri("https://github.com/igor-couto") },
                License = new OpenApiLicense {Name = "GNU General Public License V3", Url = new Uri("https://github.com/igor-couto/ahoy-chat/blob/main/LICENCE")}
            });

            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.\n Enter 'Bearer' [space] and then your token in the text input below.\nExample: \"Bearer 12345abcdef\""
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{applicationInfo.Name.Replace(" ", string.Empty)}.xml"));
        });
    }

    public static void UseSwaggerConfiguration(this WebApplication app, ApplicationInfo applicationInfo)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var title = $"{applicationInfo.Name} v{applicationInfo.VersionMajor}";
            options.DocumentTitle = title;
            options.SwaggerEndpoint($"/swagger/v{applicationInfo.VersionMajor}/swagger.json", title);
            options.DefaultModelsExpandDepth(-1);
            options.DisplayRequestDuration();
        });
    }
}