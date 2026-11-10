using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaFiscalizacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DenunciaId = table.Column<long>(type: "bigint", nullable: false),
                    VistoriaId = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrazoDias = table.Column<int>(type: "int", nullable: false),
                    DataLimite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Denuncias_DenunciaId",
                        column: x => x.DenunciaId,
                        principalTable: "Denuncias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Vistorias_VistoriaId",
                        column: x => x.VistoriaId,
                        principalTable: "Vistorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_DenunciaId",
                table: "Notificacoes",
                column: "DenunciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_VistoriaId",
                table: "Notificacoes",
                column: "VistoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificacoes");
        }
    }
}
