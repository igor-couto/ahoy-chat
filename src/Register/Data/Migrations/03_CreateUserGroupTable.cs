using FluentMigrator;
using AhoyRegister.Entities;

namespace AhoyRegister.Migrations;

[Migration(3)]
public class CreateUserGroupTable : Migration
{
    public override void Up()
    {
        Create.Table("user_group")
            .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("name")
                .AsString(128)
                .NotNullable()
            .WithColumn("number_of_members")
                .AsInt32()
                .NotNullable()
            .WithColumn("created_at")
                .AsDate()
                .NotNullable()
            .WithColumn("updated_at")
                .AsDate()
                .Nullable();
    }

    public override void Down() => Delete.Table("user_group");
}