using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Dapper;
using Scrypt;
using AhoyAuth.Requests;
using AhoyShared.Configuration;
using AhoyShared.Entities;

var builder = WebApplication.CreateBuilder(args);
var applicationInfo = builder.Configuration.GetSection("Application").Get<ApplicationInfo>();

RegisterServices(builder, applicationInfo);

await using var app = builder.Build();
ConfigureApplication(app, applicationInfo);

static void RegisterServices(WebApplicationBuilder builder, ApplicationInfo applicationInfo)
{
    builder.Services.AddCorsConfiguration();
    builder.Services.AddSwagger(applicationInfo);
}

static void ConfigureApplication(WebApplication app, ApplicationInfo applicationInfo)
{
    app.UseRouting();
    app.UseCorsConfiguration();
    app.UseSwaggerConfiguration(applicationInfo);
}

using var _dataContext = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));

var _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
var _issuer = builder.Configuration["Jwt:Issuer"];
var _audience = builder.Configuration["Jwt:Audience"];

app.MapPost("/login", async ([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken) =>
{
    var user = await FindUserFromRequest(loginRequest, cancellationToken);

    if(user is null)
        return Results.NotFound("User not found");

    var isPasswordValid = new ScryptEncoder().Compare(user.PasswordSalt + loginRequest.Password, user.PasswordHash);

    if(!isPasswordValid) return Results.Unauthorized();

    var token = new LoginResult
    {
        AccessToken = GenerateNewToken(user),
        ExpiresAfter = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH':'mm':'ss K")
    };

    var refreshToken = GenerateRefreshToken();

    SetRefreshTokenCookie(refreshToken);

    return Results.Ok(token);
});

async Task<User> FindUserFromRequest(LoginRequest loginRequest, CancellationToken cancellationToken)
{
    if(!string.IsNullOrEmpty(loginRequest.UserName))
    {       
        var userQuery = @"SELECT * FROM users WHERE user_name = @UserName";
        return await _dataContext.QuerySingleAsync(userQuery, new {UserName = loginRequest.UserName});
    }

    if(!string.IsNullOrEmpty(loginRequest.Email))
    {   
        var emailQuery = @"SELECT * FROM users WHERE email = @Email";
        return await _dataContext.QuerySingleAsync(emailQuery, new {@Email = loginRequest.Email});
    }

    return null;    
}

string GenerateNewToken(User user)
{
    var signingCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>() 
    {
        new Claim("preferred_username", user.UserName),
        new Claim("given_name", user.FirstName),
        new Claim("family_name", user.LastName),
        new Claim("phone_number", user.PhoneNumber),
        new Claim("email", user.Email),
        new Claim("role", user.Role.ToString().ToLower()),
    };

    var tokeOptions = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: signingCredentials);

    return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
}

RefreshToken GenerateRefreshToken()
{
    var refreshToken = new RefreshToken
    {
        Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
        Expires = DateTime.UtcNow.AddDays(2),
        Created = DateTime.UtcNow 
    };

    return refreshToken;
}

void SetRefreshTokenCookie(RefreshToken newRefreshToken)
{
    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Expires = newRefreshToken.Expires
    };

    // TODO:
    //Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
}

app.Run();