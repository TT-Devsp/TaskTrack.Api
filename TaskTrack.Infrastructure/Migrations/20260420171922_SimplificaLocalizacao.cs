using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaLocalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_pai_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropTable(
                name: "local_filho");

            migrationBuilder.DropTable(
                name: "local_pai");

            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_local_filho_id",
                table: "solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_local_filho_id_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropColumn(
                name: "local_filho_id",
                table: "solicitacoes");

            migrationBuilder.DropColumn(
                name: "local_pai_id",
                table: "solicitacoes");

            migrationBuilder.AddColumn<string>(
                name: "localizacao",
                table: "solicitacoes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "localizacao",
                table: "solicitacoes");

            migrationBuilder.AddColumn<Guid>(
                name: "local_filho_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "local_pai_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "local_pai",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    endereco = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_local_pai", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "local_filho",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_pai_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_local_filho", x => x.id);
                    table.UniqueConstraint("AK_local_filho_id_local_pai_id", x => new { x.id, x.local_pai_id });
                    table.ForeignKey(
                        name: "FK_local_filho_local_pai_local_pai_id",
                        column: x => x.local_pai_id,
                        principalTable: "local_pai",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_local_filho_id",
                table: "solicitacoes",
                column: "local_filho_id");

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_local_filho_id_local_pai_id",
                table: "solicitacoes",
                columns: new[] { "local_filho_id", "local_pai_id" });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_local_pai_id",
                table: "solicitacoes",
                column: "local_pai_id");

            migrationBuilder.CreateIndex(
                name: "IX_local_filho_local_pai_id",
                table: "local_filho",
                column: "local_pai_id");

            migrationBuilder.CreateIndex(
                name: "IX_local_filho_local_pai_id_nome",
                table: "local_filho",
                columns: new[] { "local_pai_id", "nome" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id_local_pai_id",
                table: "solicitacoes",
                columns: new[] { "local_filho_id", "local_pai_id" },
                principalTable: "local_filho",
                principalColumns: new[] { "id", "local_pai_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_solicitacoes_local_pai_local_pai_id",
                table: "solicitacoes",
                column: "local_pai_id",
                principalTable: "local_pai",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
