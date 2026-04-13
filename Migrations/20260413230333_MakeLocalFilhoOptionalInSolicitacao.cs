using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeLocalFilhoOptionalInSolicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id",
                table: "solicitacoes");

            migrationBuilder.AlterColumn<Guid>(
                name: "local_filho_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "local_pai_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE solicitacoes s
                  SET local_pai_id = lf.local_pai_id
                  FROM local_filho lf
                  WHERE s.local_filho_id = lf.id");

            migrationBuilder.AlterColumn<Guid>(
                name: "local_pai_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_local_filho_id_local_pai_id",
                table: "local_filho",
                columns: new[] { "id", "local_pai_id" });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_local_filho_id_local_pai_id",
                table: "solicitacoes",
                columns: new[] { "local_filho_id", "local_pai_id" });

            migrationBuilder.CreateIndex(
                name: "IX_solicitacoes_local_pai_id",
                table: "solicitacoes",
                column: "local_pai_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_solicitacoes_local_pai_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_local_filho_id_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_solicitacoes_local_pai_id",
                table: "solicitacoes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_local_filho_id_local_pai_id",
                table: "local_filho");

            migrationBuilder.DropColumn(
                name: "local_pai_id",
                table: "solicitacoes");

            migrationBuilder.AlterColumn<Guid>(
                name: "local_filho_id",
                table: "solicitacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_solicitacoes_local_filho_local_filho_id",
                table: "solicitacoes",
                column: "local_filho_id",
                principalTable: "local_filho",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
