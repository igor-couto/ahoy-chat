using System.Runtime.Serialization;

namespace AhoyRegister.Entities;

public enum Role
{
    [EnumMember(Value = "admin")] Admin,
    [EnumMember(Value = "user")] User
}