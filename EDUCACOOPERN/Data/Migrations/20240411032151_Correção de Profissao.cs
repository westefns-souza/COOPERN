using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreçãodeProfissao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Profissião",
                table: "AspNetUsers",
                newName: "Profissao");

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "AspNetUsers",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Profissao",
                table: "AspNetUsers",
                newName: "Profissião");

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "AspNetUsers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14,
                oldNullable: true);
        }
    }
}
