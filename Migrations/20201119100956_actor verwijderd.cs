using Microsoft.EntityFrameworkCore.Migrations;

namespace Uitgave_Beheer.Migrations
{
    public partial class actorverwijderd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actor",
                table: "Expenses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Actor",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
