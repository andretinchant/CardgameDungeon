using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardgameDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllyClassAndAttackerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Cards",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "Cards");
        }
    }
}
