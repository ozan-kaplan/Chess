using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Data.Migrations
{
    public partial class NewTableAdded12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chessgameanalyzequalityresults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityOfWhitePlayer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QualityOfBlackPlayer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WhiteMistakeCount = table.Column<int>(type: "int", nullable: false),
                    WhiteInaccuraciesCount = table.Column<int>(type: "int", nullable: false),
                    WhiteBlunderCount = table.Column<int>(type: "int", nullable: false),
                    BlackMistakeCount = table.Column<int>(type: "int", nullable: false),
                    BlackInaccuraciesCount = table.Column<int>(type: "int", nullable: false),
                    BlackBlunderCount = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chessgameanalyzequalityresults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chessgameanalyzeresults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameMoveNo = table.Column<int>(type: "int", nullable: false),
                    Move = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuggestedMove = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chessgameanalyzeresults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chessgameanalyzequalityresults");

            migrationBuilder.DropTable(
                name: "chessgameanalyzeresults");
        }
    }
}
