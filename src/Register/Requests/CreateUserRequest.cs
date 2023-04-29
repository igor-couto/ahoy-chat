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

    // public User Create()
    // {
    //     var passwordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 6);

    //     return new User()
    //     {
    //         Id = Guid.NewGuid(),
    //         UserName = UserName,
    //         FirstName = FirstName,
    //         LastName = LastName,
    //         Email = Email,
    //         PhoneNumber = PhoneNumber,
    //         IsEmailConfirmed = false,
    //         IsLocked = false,
    //         CreatedAt = DateTime.Now,
    //         PasswordSalt = passwordSalt,
    //         PasswordHash = new ScryptEncoder().Encode(passwordSalt + Password),
    //         Role = Role.User
    //     };
    // }
}