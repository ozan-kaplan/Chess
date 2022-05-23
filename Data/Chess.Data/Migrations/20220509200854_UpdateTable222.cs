using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Data.Migrations
{
    public partial class UpdateTable222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoveNo",
                table: "chessgamemoves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveNo",
                table: "chessgamemoves");
        }
    }
}
