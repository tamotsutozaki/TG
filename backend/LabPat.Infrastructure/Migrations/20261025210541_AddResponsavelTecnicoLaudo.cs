using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsavelTecnicoLaudo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponsavelTecnicoCrmvEstado",
                table: "Laudos",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponsavelTecnicoCrmvNumero",
                table: "Laudos",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponsavelTecnicoNome",
                table: "Laudos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsavelTecnicoCrmvEstado",
                table: "Laudos");

            migrationBuilder.DropColumn(
                name: "ResponsavelTecnicoCrmvNumero",
                table: "Laudos");

            migrationBuilder.DropColumn(
                name: "ResponsavelTecnicoNome",
                table: "Laudos");
        }
    }
}
