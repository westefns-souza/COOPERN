using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class PDIAreasDeAtuacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PDIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDIs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PDIAreaAtuacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PDIId = table.Column<int>(type: "integer", nullable: false),
                    AreaAtuacaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDIAreaAtuacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PDIAreaAtuacoes_AreaAtuacao_AreaAtuacaoId",
                        column: x => x.AreaAtuacaoId,
                        principalTable: "AreaAtuacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PDIAreaAtuacoes_PDIs_PDIId",
                        column: x => x.PDIId,
                        principalTable: "PDIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PDIAreaAtuacoes_AreaAtuacaoId",
                table: "PDIAreaAtuacoes",
                column: "AreaAtuacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PDIAreaAtuacoes_PDIId",
                table: "PDIAreaAtuacoes",
                column: "PDIId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PDIAreaAtuacoes");

            migrationBuilder.DropTable(
                name: "PDIs");
        }
    }
}
