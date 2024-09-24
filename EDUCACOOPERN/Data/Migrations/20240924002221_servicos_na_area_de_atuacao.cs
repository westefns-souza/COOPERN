using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class servicos_na_area_de_atuacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicosId",
                table: "UsuarioAreaAtuacao",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAreaAtuacao_ServicosId",
                table: "UsuarioAreaAtuacao",
                column: "ServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioAreaAtuacao_Servicos_ServicosId",
                table: "UsuarioAreaAtuacao",
                column: "ServicosId",
                principalTable: "Servicos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioAreaAtuacao_Servicos_ServicosId",
                table: "UsuarioAreaAtuacao");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioAreaAtuacao_ServicosId",
                table: "UsuarioAreaAtuacao");

            migrationBuilder.DropColumn(
                name: "ServicosId",
                table: "UsuarioAreaAtuacao");
        }
    }
}
