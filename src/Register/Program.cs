using AhoyShared.Configuration;
using AhoyRegister.Configuration;
using AhoyRegister.Requests;
using Microsoft.AspNetCore.Http.Features;
using Npgsql;
using Dapper;
using Scrypt;
using AhoyShared.Entities;

var builder = WebApplication.CreateBuilder(args);
var applicationInfo = builder.Configuration.GetSection("Application").Get<ApplicationInfo>();

RegisterServices(builder, applicationInfo);

await using var app = builder.Build();
ConfigureApplication(app, applicationInfo);


static void RegisterServices(WebApplicationBuilder builder, ApplicationInfo applicationInfo)
{
    builder.Services.AddCorsConfiguration();

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
        options.MemoryBufferThreshold = int.MaxValue;
    });

    builder.Services.AddSwagger(applicationInfo);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.RegisterFluentMigrator(connectionString);

    builder.Services.AddHttpContextAccessor();
}


static void ConfigureApplication(WebApplication app, ApplicationInfo applicationInfo)
{
    app.UseRouting();
    app.UseCorsConfiguration();   
    app.UseFluentMigratorConfiguration(); 
    app.UseSwaggerConfiguration(applicationInfo);
}

app.MapPost("/users", async (HttpContext context, IConfiguration configuration) =>
{
    if (!context.Request.HasFormContentType)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { message = "Invalid request format." });
        return;
    }

    var form = await context.Request.ReadFormAsync();
    var createUserRequest = new CreateUserRequest
    {
        UserName = form["UserName"],
        FirstName = form["FirstName"],
        LastName = form["LastName"],
        Email = form["Email"],
        PhoneNumber = form["PhoneNumber"],
        Password = form["Password"],
        //UserGroups = form["UserGroups"].ToString().Split(',').ToArray()
    };

    // Validate the request (e.g., check for required fields, password constraints, etc.)

    // Handle the uploaded image
    var image = form.Files.GetFile("Photo");
    if (image != null && image.Length > 0)
    {
        // Save the image to your desired location (e.g., local file system, cloud storage, etc.)
        // You can use image.OpenReadStream() to read the file stream and process it accordingly
    }

    // Save the user to the database (you need to implement this according to your database setup)

    await SaveUser(createUserRequest, image, configuration);


    // Return a success response
    await context.Response.WriteAsJsonAsync(new { message = "User created successfully." });

    // var url = $"{Request.Scheme}://{Request.Host.Value}/user/{user.Id}";
    // return Created(url, user);
});



static async Task SaveUser(CreateUserRequest user, IFormFile image, IConfiguration configuration)
{
    using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));

    var userId = Guid.NewGuid();

    var passwordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 6);
    var passwordHash = new ScryptEncoder().Encode(passwordSalt + user.Password);

    var role = Role.User;

    // Save the user data in the "users" table
    const string userInsertQuery = @"
        INSERT INTO users (id, user_name, first_name, last_name, email, phone_number, password_hash, password_salt, role, created_at)
        VALUES (@Id, @UserName, @FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @PasswordSalt, @Role, @CreatedAt)
        RETURNING id;";

    await connection.QuerySingleAsync(userInsertQuery, new { 
        Id = userId,
        UserName = user.UserName,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        PasswordHash = passwordHash,
        PasswordSalt = passwordSalt,
        Role = role,
        CreatedAt = DateTime.UtcNow
    });

    // Save the user groups in the "user_groups" table
    //const string userGroupInsertQuery = @"
    //    INSERT INTO user_groups (user_id, group_id)
    //    VALUES (@UserId, @GroupId);";

    //foreach (var groupId in user.UserGroups)
    //{
    //    await connection.ExecuteAsync(userGroupInsertQuery, new { UserId = userId, GroupId = groupId });
    //}

    // Save the user image (you can store the image bytes in the database or save the path to the image file)
    if (image != null && image.Length > 0)
    {
        // Example: Saving the image bytes in the "user_images" table
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();

        const string imageInsertQuery = @"
            INSERT INTO user_profile_pictures (user_id, image_data)
            VALUES (@UserId, @ImageData);";

        await connection.ExecuteAsync(imageInsertQuery, new { UserId = userId, ImageData = imageBytes });
    }
}

app.Run();