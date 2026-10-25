using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabPat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiLaboratorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_VetsSolicitantes_VetSolicitanteId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "StatusConta",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_VetSolicitanteId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VetSolicitanteId",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_LaboratorioId",
                table: "Usuarios",
                column: "LaboratorioId");

            migrationBuilder.AlterColumn<string>(
                name: "CrmvNumero",
                table: "VetsSolicitantes",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CrmvEstado",
                table: "VetsSolicitantes",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "VetsSolicitantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CrmvEstado",
                table: "Usuarios",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrmvNumero",
                table: "Usuarios",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Tutores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "TiposExame",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CriadoPorId",
                table: "Solicitacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Solicitacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Pacientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Insumos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Laboratorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsavelTecnicoNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsavelTecnicoCrmvNumero = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ResponsavelTecnicoCrmvEstado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CodigoVinculo = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratorios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConvitesLaboratorio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiraEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevogadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoPorId = table.Column<int>(type: "int", nullable: false),
                    LaboratorioId = table.Column<int>(type: "int", nullable: true),
                    Versao = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvitesLaboratorio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvitesLaboratorio_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvitesLaboratorio_Usuarios_CriadoPorId",
                        column: x => x.CriadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VinculosVeterinario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SolicitadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondidoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VeterinarioId = table.Column<int>(type: "int", nullable: false),
                    LaboratorioId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinculosVeterinario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinculosVeterinario_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VinculosVeterinario_Usuarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VetsSolicitantes_LaboratorioId_CrmvNumero_CrmvEstado",
                table: "VetsSolicitantes",
                columns: new[] { "LaboratorioId", "CrmvNumero", "CrmvEstado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CrmvNumero_CrmvEstado",
                table: "Usuarios",
                columns: new[] { "CrmvNumero", "CrmvEstado" },
                unique: true,
                filter: "[CrmvNumero] IS NOT NULL AND [Ativo] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_LaboratorioId",
                table: "Tutores",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposExame_LaboratorioId",
                table: "TiposExame",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_CriadoPorId",
                table: "Solicitacoes",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_LaboratorioId",
                table: "Solicitacoes",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_LaboratorioId",
                table: "Pacientes",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_LaboratorioId",
                table: "Insumos",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesLaboratorio_CriadoPorId",
                table: "ConvitesLaboratorio",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesLaboratorio_LaboratorioId",
                table: "ConvitesLaboratorio",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesLaboratorio_TokenHash",
                table: "ConvitesLaboratorio",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_Cnpj",
                table: "Laboratorios",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_CodigoVinculo",
                table: "Laboratorios",
                column: "CodigoVinculo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VinculosVeterinario_LaboratorioId",
                table: "VinculosVeterinario",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosVeterinario_VeterinarioId_LaboratorioId",
                table: "VinculosVeterinario",
                columns: new[] { "VeterinarioId", "LaboratorioId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Insumos_Laboratorios_LaboratorioId",
                table: "Insumos",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_Laboratorios_LaboratorioId",
                table: "Pacientes",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitacoes_Laboratorios_LaboratorioId",
                table: "Solicitacoes",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitacoes_Usuarios_CriadoPorId",
                table: "Solicitacoes",
                column: "CriadoPorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposExame_Laboratorios_LaboratorioId",
                table: "TiposExame",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_Laboratorios_LaboratorioId",
                table: "Tutores",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Laboratorios_LaboratorioId",
                table: "Usuarios",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VetsSolicitantes_Laboratorios_LaboratorioId",
                table: "VetsSolicitantes",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insumos_Laboratorios_LaboratorioId",
                table: "Insumos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_Laboratorios_LaboratorioId",
                table: "Pacientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Solicitacoes_Laboratorios_LaboratorioId",
                table: "Solicitacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Solicitacoes_Usuarios_CriadoPorId",
                table: "Solicitacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposExame_Laboratorios_LaboratorioId",
                table: "TiposExame");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_Laboratorios_LaboratorioId",
                table: "Tutores");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Laboratorios_LaboratorioId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_VetsSolicitantes_Laboratorios_LaboratorioId",
                table: "VetsSolicitantes");

            migrationBuilder.DropTable(
                name: "ConvitesLaboratorio");

            migrationBuilder.DropTable(
                name: "VinculosVeterinario");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropIndex(
                name: "IX_VetsSolicitantes_LaboratorioId_CrmvNumero_CrmvEstado",
                table: "VetsSolicitantes");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CrmvNumero_CrmvEstado",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Tutores_LaboratorioId",
                table: "Tutores");

            migrationBuilder.DropIndex(
                name: "IX_TiposExame_LaboratorioId",
                table: "TiposExame");

            migrationBuilder.DropIndex(
                name: "IX_Solicitacoes_CriadoPorId",
                table: "Solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_Solicitacoes_LaboratorioId",
                table: "Solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_LaboratorioId",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Insumos_LaboratorioId",
                table: "Insumos");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "VetsSolicitantes");

            migrationBuilder.DropColumn(
                name: "CrmvEstado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CrmvNumero",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "TiposExame");

            migrationBuilder.DropColumn(
                name: "CriadoPorId",
                table: "Solicitacoes");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Solicitacoes");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Insumos");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_LaboratorioId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "VetSolicitanteId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_VetSolicitanteId",
                table: "Usuarios",
                column: "VetSolicitanteId");

            migrationBuilder.AlterColumn<string>(
                name: "CrmvNumero",
                table: "VetsSolicitantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "CrmvEstado",
                table: "VetsSolicitantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<int>(
                name: "StatusConta",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_VetsSolicitantes_VetSolicitanteId",
                table: "Usuarios",
                column: "VetSolicitanteId",
                principalTable: "VetsSolicitantes",
                principalColumn: "Id");
        }
    }
}
