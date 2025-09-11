using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnLastUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdate",
                schema: "ticker",
                table: "Intervals",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "ticker",
                table: "Intervals");
        }
    }
}
