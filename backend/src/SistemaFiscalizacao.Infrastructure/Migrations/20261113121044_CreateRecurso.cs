using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaFiscalizacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateRecurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recursos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessoInfracaoId = table.Column<long>(type: "bigint", nullable: false),
                    Justificativa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataApresentacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Decisao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataDecisao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recursos_ProcessosInfracao_ProcessoInfracaoId",
                        column: x => x.ProcessoInfracaoId,
                        principalTable: "ProcessosInfracao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recursos_ProcessoInfracaoId",
                table: "Recursos",
                column: "ProcessoInfracaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recursos");
        }
    }
}
