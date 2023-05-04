using System.Runtime.Serialization;

namespace AhoyShared.Entities;

public enum Role
{
    [EnumMember(Value = "admin")] Admin,
    [EnumMember(Value = "user")] User
}