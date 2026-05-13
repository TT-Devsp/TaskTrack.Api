using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResolutionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataLimite",
                table: "solicitacoes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Prioridade",
                table: "solicitacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataLimite",
                table: "solicitacoes");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "solicitacoes");
        }
    }
}
