using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardgameDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEffectTagsToCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EffectTags",
                table: "Cards",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectTags",
                table: "Cards");
        }
    }
}
