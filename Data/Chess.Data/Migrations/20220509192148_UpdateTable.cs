using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Data.Migrations
{
    public partial class UpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player1Name",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Id",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Name",
                table: "chessgames");
        }
    }
}
