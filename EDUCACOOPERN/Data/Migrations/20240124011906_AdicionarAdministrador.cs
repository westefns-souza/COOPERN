using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var id = Guid.NewGuid().ToString();
            var userName = "administrador@educacoopern.com";
            var email = "administrador@educacoopern.com";

            migrationBuilder.InsertData(
               table: "AspNetUsers",
               columns: new[] {       "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount" },
               values: new object[] { id, userName, userName.ToUpper(), email, email.ToUpper(), true, "AQAAAAIAAYagAAAAEOyYcOSWPisMYoRImW7qUlKnWORpHxx12bPfrXc7ry8OEyOeCqVVAJuYMouJG6J+kQ==", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, false, true, 0 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { id, "1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
