namespace AhoyRegister.Entities;

public record UserGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int NumberOfMembers { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}