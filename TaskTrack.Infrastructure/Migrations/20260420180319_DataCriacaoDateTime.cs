using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataCriacaoDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "solicitacoes",
                type: "timestamp(0) without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "data_criacao",
                table: "solicitacoes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(0) without time zone");
        }
    }
}
