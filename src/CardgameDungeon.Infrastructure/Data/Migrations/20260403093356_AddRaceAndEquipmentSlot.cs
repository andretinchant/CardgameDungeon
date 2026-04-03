using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardgameDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceAndEquipmentSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Race",
                table: "Cards",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slot",
                table: "Cards",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Race",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Slot",
                table: "Cards");
        }
    }
}
