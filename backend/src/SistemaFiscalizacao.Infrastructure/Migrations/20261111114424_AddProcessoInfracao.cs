using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaFiscalizacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessoInfracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessosInfracao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DenunciaId = table.Column<long>(type: "bigint", nullable: false),
                    VistoriaId = table.Column<long>(type: "bigint", nullable: false),
                    NumeroProcesso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValorMulta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessosInfracao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessosInfracao_Denuncias_DenunciaId",
                        column: x => x.DenunciaId,
                        principalTable: "Denuncias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessosInfracao_Vistorias_VistoriaId",
                        column: x => x.VistoriaId,
                        principalTable: "Vistorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessosInfracao_DenunciaId",
                table: "ProcessosInfracao",
                column: "DenunciaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessosInfracao_VistoriaId",
                table: "ProcessosInfracao",
                column: "VistoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessosInfracao");
        }
    }
}
