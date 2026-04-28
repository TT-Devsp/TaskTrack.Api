using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGestorResponsavelIdToSolicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "gestor_responsavel_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_gestor_responsavel_id",
                table: "solicitacoes",
                column: "gestor_responsavel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_gestor_responsavel_id",
                table: "solicitacoes");

            migrationBuilder.DropColumn(
                name: "gestor_responsavel_id",
                table: "solicitacoes");
        }
    }
}
