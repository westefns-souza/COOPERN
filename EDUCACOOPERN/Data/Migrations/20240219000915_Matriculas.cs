using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class Matriculas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matricula",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AulaId = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<string>(type: "text", nullable: false),
                    NotaPre = table.Column<decimal>(type: "numeric", nullable: false),
                    NotaPos = table.Column<decimal>(type: "numeric", nullable: false),
                    Compareceu = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matricula", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matricula_AspNetUsers_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matricula_Aulas_AulaId",
                        column: x => x.AulaId,
                        principalTable: "Aulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_AlunoId",
                table: "Matricula",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_AulaId",
                table: "Matricula",
                column: "AulaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matricula");
        }
    }
}
