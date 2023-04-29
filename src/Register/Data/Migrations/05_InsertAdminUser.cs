using FluentMigrator;
using AhoyRegister.Entities;

namespace AhoyRegister.Migrations;

[Migration(5)]
public class CreateAdminUser : Migration
{
    public override void Up()
    {
        Insert.IntoTable("users").Row(new {
            id = "94064f7b-9930-4ef1-b754-79e7aefe1e01",
            user_name = "admin",
            first_name = "admin",
            last_name = "admin",
            email = "admin@admin.com",
            is_locked = "false",
            phone_number = "1234567890",
            created_at = DateTime.Now,
            password_hash = "$s2$16384$8$1$MXDdcD5jOFVUNHWsifoDUCTvBQjXmrb4/h9OdWbN89s=$YR6MG7Y+qWMDw/jwQGlFe2qQ/VtWmuuE0id+SF6mq5o=",
            password_salt = "000000",
            role = (short) Role.Admin
        });
    }

    public override void Down() 
        => Delete.FromTable("users").Row(new { id = "94064f7b-9930-4ef1-b754-79e7aefe1e01" });
}