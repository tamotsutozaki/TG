using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPapelEStatusConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Papel",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusConta",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VetSolicitanteId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            // Usuários existentes antes desta feature são patologistas e já aprovados
            migrationBuilder.Sql("UPDATE Usuarios SET StatusConta = 1;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_VetSolicitanteId",
                table: "Usuarios",
                column: "VetSolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_VetsSolicitantes_VetSolicitanteId",
                table: "Usuarios",
                column: "VetSolicitanteId",
                principalTable: "VetsSolicitantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_VetsSolicitantes_VetSolicitanteId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_VetSolicitanteId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Papel",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "StatusConta",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VetSolicitanteId",
                table: "Usuarios");
        }
    }
}
