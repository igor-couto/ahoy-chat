using FluentMigrator;

namespace AhoyRegister.Migrations;

[Migration(1)]
public class CreateRoleTable : Migration
{
    public override void Up()
    {
        Create.Table("role")
            .WithColumn("id")
                .AsByte()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("role")
                .AsString(128)
                .NotNullable();
    }

    public override void Down() => Delete.Table("role");
}