using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDUCACOOPERN.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModificacoesNasMatriculas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Custos_Aulas_AulaId",
                table: "Custos");

            migrationBuilder.DropForeignKey(
                name: "FK_Custos_Cursos_CursoId",
                table: "Custos");

            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_AspNetUsers_AlunoId",
                table: "Matricula");

            migrationBuilder.DropIndex(
                name: "IX_Custos_CursoId",
                table: "Custos");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "Custos");

            migrationBuilder.AlterColumn<string>(
                name: "AlunoId",
                table: "Matricula",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Matricula",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AulaId",
                table: "Custos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Custos_Aulas_AulaId",
                table: "Custos",
                column: "AulaId",
                principalTable: "Aulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_AspNetUsers_AlunoId",
                table: "Matricula",
                column: "AlunoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Custos_Aulas_AulaId",
                table: "Custos");

            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_AspNetUsers_AlunoId",
                table: "Matricula");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Matricula");

            migrationBuilder.AlterColumn<string>(
                name: "AlunoId",
                table: "Matricula",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AulaId",
                table: "Custos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "Custos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Custos_CursoId",
                table: "Custos",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Custos_Aulas_AulaId",
                table: "Custos",
                column: "AulaId",
                principalTable: "Aulas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Custos_Cursos_CursoId",
                table: "Custos",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_AspNetUsers_AlunoId",
                table: "Matricula",
                column: "AlunoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
