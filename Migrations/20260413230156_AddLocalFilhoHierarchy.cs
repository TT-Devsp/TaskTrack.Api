using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalFilhoHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_pai_local_id",
                table: "solicitacoes");

            migrationBuilder.RenameColumn(
                name: "local_id",
                table: "solicitacoes",
                newName: "local_filho_id");

            migrationBuilder.RenameIndex(
                name: "IX_solicitacoes_local_id",
                table: "solicitacoes",
                newName: "IX_solicitacoes_local_filho_id");

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
                    table.ForeignKey(
                        name: "FK_local_filho_local_pai_local_pai_id",
                        column: x => x.local_pai_id,
                        principalTable: "local_pai",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_local_filho_local_pai_id",
                table: "local_filho",
                column: "local_pai_id");

            migrationBuilder.CreateIndex(
                name: "IX_local_filho_local_pai_id_nome",
                table: "local_filho",
                columns: new[] { "local_pai_id", "nome" },
                unique: true);

            // Backfill one child location per parent location to preserve existing references.
            migrationBuilder.Sql(
                @"INSERT INTO local_filho (id, local_pai_id, nome)
                  SELECT id, id, nome
                  FROM local_pai");

            migrationBuilder.AddForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id",
                table: "solicitacoes",
                column: "local_filho_id",
                principalTable: "local_filho",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id",
                table: "solicitacoes");

            migrationBuilder.Sql(
                @"UPDATE solicitacoes s
                  SET local_filho_id = lf.local_pai_id
                  FROM local_filho lf
                  WHERE s.local_filho_id = lf.id");

            migrationBuilder.DropTable(
                name: "local_filho");

            migrationBuilder.RenameColumn(
                name: "local_filho_id",
                table: "solicitacoes",
                newName: "local_id");

            migrationBuilder.RenameIndex(
                name: "IX_solicitacoes_local_filho_id",
                table: "solicitacoes",
                newName: "IX_solicitacoes_local_id");

            migrationBuilder.AddForeignKey(
                name: "FK_solicitacoes_local_pai_local_id",
                table: "solicitacoes",
                column: "local_id",
                principalTable: "local_pai",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
