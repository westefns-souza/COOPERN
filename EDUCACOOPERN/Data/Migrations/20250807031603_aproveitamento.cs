using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class aproveitamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Aproveitamento",
                table: "Matricula",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CargaHorariaPratica",
                table: "Cursos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CargaHorariaTeorica",
                table: "Cursos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aproveitamento",
                table: "Matricula");

            migrationBuilder.DropColumn(
                name: "CargaHorariaPratica",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "CargaHorariaTeorica",
                table: "Cursos");
        }
    }
}
