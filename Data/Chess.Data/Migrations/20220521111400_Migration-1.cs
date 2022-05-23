using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Data.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Player1Score",
                table: "chessgames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Score",
                table: "chessgames",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Score",
                table: "chessgames");

            migrationBuilder.DropColumn(
                name: "Player2Score",
                table: "chessgames");
        }
    }
}
