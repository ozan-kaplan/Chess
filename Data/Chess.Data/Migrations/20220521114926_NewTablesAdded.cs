using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Data.Migrations
{
    public partial class NewTablesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Color",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player1Name",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player1Score",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Color",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Id",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Name",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Score",
                table: "chessgames");

            migrationBuilder.CreateTable(
                name: "chessgameplayers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chessgameplayers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chessgameplayers");

            migrationBuilder.AddColumn<string>(
                name: "Player1Color",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player1Id",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player1Name",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player1Score",
                table: "chessgames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Player2Color",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player2Id",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player2Name",
                table: "chessgames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2Score",
                table: "chessgames",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
