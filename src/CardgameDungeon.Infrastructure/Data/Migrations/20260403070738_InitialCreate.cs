using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardgameDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeckLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BossCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdventurerCardIds = table.Column<List<Guid>>(type: "jsonb", nullable: false),
                    EnemyCardIds = table.Column<List<Guid>>(type: "jsonb", nullable: false),
                    DungeonRoomCardIds = table.Column<List<Guid>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnedCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceListings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phase = table.Column<int>(type: "integer", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    StateJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnedCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsReserved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Elo = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerWallets",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<int>(type: "integer", nullable: false),
                    LastDailyRewardDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DailyRewardsClaimedToday = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerWallets", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "QueueEntries",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    QueueType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Elo = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueEntries", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredTier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EntryFee = table.Column<int>(type: "integer", nullable: false),
                    PrizePool = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentRound = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Rarity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false),
                    CardSetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CardType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Strength = table.Column<int>(type: "integer", nullable: true),
                    HitPoints = table.Column<int>(type: "integer", nullable: true),
                    Initiative = table.Column<int>(type: "integer", nullable: true),
                    Treasure = table.Column<int>(type: "integer", nullable: true),
                    IsAmbusher = table.Column<bool>(type: "boolean", nullable: true),
                    Effect = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    MonsterCostBudget = table.Column<int>(type: "integer", nullable: true),
                    MonsterIds = table.Column<IReadOnlyList<Guid>>(type: "jsonb", nullable: true),
                    TrapIds = table.Column<IReadOnlyList<Guid>>(type: "jsonb", nullable: true),
                    StrengthModifier = table.Column<int>(type: "integer", nullable: true),
                    HitPointsModifier = table.Column<int>(type: "integer", nullable: true),
                    InitiativeModifier = table.Column<int>(type: "integer", nullable: true),
                    Damage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_CardSets_CardSetId",
                        column: x => x.CardSetId,
                        principalTable: "CardSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TournamentParticipants",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEliminated = table.Column<bool>(type: "boolean", nullable: false),
                    EliminatedInRound = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentParticipants", x => new { x.TournamentId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardSetId",
                table: "Cards",
                column: "CardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardType",
                table: "Cards",
                column: "CardType");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Rarity",
                table: "Cards",
                column: "Rarity");

            migrationBuilder.CreateIndex(
                name: "IX_CardSets_Code",
                table: "CardSets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeckLists_PlayerId",
                table: "DeckLists",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_IsActive",
                table: "MarketplaceListings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_SellerId",
                table: "MarketplaceListings",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchStates_WinnerId",
                table: "MatchStates",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedCards_CardId",
                table: "OwnedCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedCards_PlayerId",
                table: "OwnedCards",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_Elo",
                table: "PlayerRatings",
                column: "Elo");

            migrationBuilder.CreateIndex(
                name: "IX_QueueEntries_QueueType",
                table: "QueueEntries",
                column: "QueueType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "DeckLists");

            migrationBuilder.DropTable(
                name: "MarketplaceListings");

            migrationBuilder.DropTable(
                name: "MatchStates");

            migrationBuilder.DropTable(
                name: "OwnedCards");

            migrationBuilder.DropTable(
                name: "PlayerRatings");

            migrationBuilder.DropTable(
                name: "PlayerWallets");

            migrationBuilder.DropTable(
                name: "QueueEntries");

            migrationBuilder.DropTable(
                name: "TournamentParticipants");

            migrationBuilder.DropTable(
                name: "CardSets");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
