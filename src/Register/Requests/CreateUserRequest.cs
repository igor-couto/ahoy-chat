using Scrypt;

namespace AhoyRegister.Requests;

public class CreateUserRequest
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string[] UserGroups { get; set; }
}