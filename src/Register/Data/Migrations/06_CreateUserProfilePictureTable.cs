using FluentMigrator;

namespace AhoyRegister.Migrations;

[Migration(6)]
public class CreateUserProfilePictureTable : Migration
{
    public override void Up()
    {
        Create.Table("user_profile_pictures")
            .WithColumn("user_id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
                .ForeignKey("users", "id")
            .WithColumn("image_data")
                .AsBinary()
                .NotNullable();
    }

    public override void Down() => Delete.Table("user_profile_pictures");
}