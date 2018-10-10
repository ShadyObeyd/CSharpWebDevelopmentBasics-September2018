using Microsoft.EntityFrameworkCore.Migrations;

namespace IRunes.Data.Migrations
{
    public partial class FixedAlbumPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Albums",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Albums");
        }
    }
}
