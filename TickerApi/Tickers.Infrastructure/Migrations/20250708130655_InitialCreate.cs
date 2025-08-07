using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ticker");

            migrationBuilder.CreateTable(
                name: "Tickers",
                schema: "ticker",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickers", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "Intervals",
                schema: "ticker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IntervalType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervals_Tickers_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalSchema: "ticker",
                        principalTable: "Tickers",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candles",
                schema: "ticker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Open = table.Column<float>(type: "real", nullable: false),
                    High = table.Column<float>(type: "real", nullable: false),
                    Low = table.Column<float>(type: "real", nullable: false),
                    Close = table.Column<float>(type: "real", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false),
                    Dividends = table.Column<float>(type: "real", nullable: false),
                    StockSplits = table.Column<float>(type: "real", nullable: false),
                    IntervalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candles_Intervals_IntervalId",
                        column: x => x.IntervalId,
                        principalSchema: "ticker",
                        principalTable: "Intervals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candles_Date",
                schema: "ticker",
                table: "Candles",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_IntervalId",
                schema: "ticker",
                table: "Candles",
                column: "IntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_IntervalType",
                schema: "ticker",
                table: "Intervals",
                column: "IntervalType");

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_TickerSymbol",
                schema: "ticker",
                table: "Intervals",
                column: "TickerSymbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candles",
                schema: "ticker");

            migrationBuilder.DropTable(
                name: "Intervals",
                schema: "ticker");

            migrationBuilder.DropTable(
                name: "Tickers",
                schema: "ticker");
        }
    }
}
