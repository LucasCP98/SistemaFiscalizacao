using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaFiscalizacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroVistoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroVistoria",
                table: "Vistorias",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroVistoria",
                table: "Vistorias");
        }
    }
}
