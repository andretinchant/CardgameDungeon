using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardgameDungeon.Unity.Network
{
    // ──────────────────────────────────────────────
    //  Shared / Error
    // ──────────────────────────────────────────────

    [Serializable]
    public class ErrorResponse
    {
        public string title;
        public int status;
        public string detail;
        public string type;
    }

    // ──────────────────────────────────────────────
    //  Deck
    // ──────────────────────────────────────────────

    [Serializable]
    public class DeckResponse
    {
        public string id;
        public string playerId;
        public List<CardDto> adventurerCards;
        public List<CardDto> enemyCards;
        public List<DeckDungeonRoomDto> dungeonRooms;
        public CardDto boss;
    }

    [Serializable]
    public class CardDto
    {
        public string id;
        public string name;
        public string type;
        public string rarity;
        public int cost;
    }

    [Serializable]
    public class DeckDungeonRoomDto
    {
        public string id;
        public string name;
        public int order;
        public List<string> monsterIds;
        public List<string> trapIds;
    }

    // ──────────────────────────────────────────────
    //  Match
    // ──────────────────────────────────────────────

    [Serializable]
    public class MatchResponse
    {
        public string id;
        public string phase;
        public int currentRoom;
        public PlayerStateDto player1;
        public PlayerStateDto player2;
        public string initiativeWinnerId;
        public string winnerId;
        public DungeonRoomMatchDto currentDungeonRoom;
        public bool isBossRoom;
    }

    [Serializable]
    public class PlayerStateDto
    {
        public string playerId;
        public int hitPoints;
        public int deckCount;
        public int handCount;
        public int discardCount;
        public int exileCount;
        public List<AllyDto> alliesInPlay;
    }

    [Serializable]
    public class AllyDto
    {
        public string id;
        public string name;
        public int strength;
        public int hitPoints;
        public int initiative;
        public int cost;
        public bool isAmbusher;
    }

    [Serializable]
    public class DungeonRoomMatchDto
    {
        public string id;
        public string name;
        public int order;
        public bool hasMonsters;
    }

    // ──────────────────────────────────────────────
    //  Setup / Initiative / Betting
    // ──────────────────────────────────────────────

    [Serializable]
    public class SetupInitialTeamResponse
    {
        public string matchId;
        public string playerId;
        public bool submitted;
        public bool bothReady;
    }

    [Serializable]
    public class InitiativeResponse
    {
        public string matchId;
        public int player1Total;
        public int player2Total;
        public string winnerId;
        public bool isTied;
    }

    [Serializable]
    public class PlaceBetResponse
    {
        public string matchId;
        public string playerId;
        public int player1BetTotal;
        public int player2BetTotal;
        public bool resolved;
        public string winnerId;
    }

    // ──────────────────────────────────────────────
    //  Combat
    // ──────────────────────────────────────────────

    [Serializable]
    public class CombatPairing
    {
        public string attackerAllyId;
        public string defenderAllyId;
    }

    [Serializable]
    public class AssignCombatResponse
    {
        public string matchId;
        public int totalAssignments;
        public List<CombatPairing> pairings;
    }

    [Serializable]
    public class CombatResultDto
    {
        public string attackerId;
        public string defenderId;
        public int attackerStrength;
        public int defenderStrength;
        public int damageToAttacker;
        public int damageToDefender;
        public string outcome;
        public bool attackerEliminated;
        public bool defenderEliminated;
    }

    [Serializable]
    public class ResolveCombatRoundResponse
    {
        public string matchId;
        public List<CombatResultDto> results;
        public string overallOutcome;
        public bool simultaneousElimination;
        public string phase;
    }

    [Serializable]
    public class RetargetResponse
    {
        public string matchId;
        public string allyId;
        public int primaryDamageContribution;
        public int secondaryDamageContribution;
        public int costPaid;
    }

    [Serializable]
    public class OpportunityAttackResponse
    {
        public string matchId;
        public string attackerAllyId;
        public string fleeingAllyId;
        public int damage;
    }

    // ──────────────────────────────────────────────
    //  Collection
    // ──────────────────────────────────────────────

    [Serializable]
    public class CollectionResponse
    {
        public string playerId;
        public List<OwnedCardDto> cards;
        public int totalCards;
        public int availableCards;
    }

    [Serializable]
    public class OwnedCardDto
    {
        public string ownedCardId;
        public string cardId;
        public bool isReserved;
        public string cardName;
        public string cardType;
        public string rarity;
        public int cost;
        public string detailText;
    }

    [Serializable]
    public class OpenBoosterResponse
    {
        public string playerId;
        public string setCode;
        public List<BoosterCardDto> cards;
    }

    [Serializable]
    public class BoosterCardDto
    {
        public string cardId;
        public string name;
        public string rarity;
        public string type;
        public string setCode;
    }

    [Serializable]
    public class BoosterSetsResponse
    {
        public List<BoosterSetDto> sets;
    }

    [Serializable]
    public class BoosterSetDto
    {
        public string setId;
        public string setCode;
        public string setName;
        public string description;
        public string releaseDate;
        public int boosterPrice;
        public int totalCards;
    }

    // ──────────────────────────────────────────────
    //  Wallet
    // ──────────────────────────────────────────────

    [Serializable]
    public class BalanceResponse
    {
        public string playerId;
        public int balance;
    }

    [Serializable]
    public class AddFundsResponse
    {
        public string playerId;
        public int newBalance;
        public string source;
    }

    // ──────────────────────────────────────────────
    //  Marketplace
    // ──────────────────────────────────────────────

    [Serializable]
    public class ListingDto
    {
        public string listingId;
        public string sellerId;
        public string cardId;
        public int price;
        public int fee;
        public bool isActive;
    }

    [Serializable]
    public class BuyCardResponse
    {
        public ListingDto listing;
        public int amountPaid;
        public int sellerReceived;
        public int feePaid;
    }

    [Serializable]
    public class GetMarketplaceResponse
    {
        public List<ListingDto> listings;
        public int totalCount;
    }

    [Serializable]
    public class CancelListingResponse
    {
        public string listingId;
        public bool cancelled;
    }

    // ──────────────────────────────────────────────
    //  Queue / Matchmaking
    // ──────────────────────────────────────────────

    [Serializable]
    public class JoinQueueResponse
    {
        public string playerId;
        public string queueType;
        public bool joined;
    }

    [Serializable]
    public class LeaveQueueResponse
    {
        public string playerId;
        public bool left;
    }

    [Serializable]
    public class FindMatchResponse
    {
        public bool matchFound;
        public string matchId;
        public string player1Id;
        public string player2Id;
    }

    // ──────────────────────────────────────────────
    //  Ranking / Elo
    // ──────────────────────────────────────────────

    [Serializable]
    public class PlayerRankResponse
    {
        public string playerId;
        public int elo;
        public string tier;
        public int wins;
        public int losses;
        public int rank;
    }

    [Serializable]
    public class UpdateEloResponse
    {
        public string winnerId;
        public int winnerNewElo;
        public int winnerDelta;
        public string loserId;
        public int loserNewElo;
        public int loserDelta;
    }

    [Serializable]
    public class RecalculateTiersResponse
    {
        public int totalPlayers;
        public int bronzeCount;
        public int silverCount;
        public int goldCount;
        public int bronzeCutoff;
        public int goldCutoff;
    }

    // ──────────────────────────────────────────────
    //  Tournament
    // ──────────────────────────────────────────────

    [Serializable]
    public class TournamentResponse
    {
        public string id;
        public string requiredTier;
        public int entryFee;
        public int prizePool;
        public string status;
        public int participantCount;
        public int currentRound;
    }

    [Serializable]
    public class JoinTournamentResponse
    {
        public string tournamentId;
        public string playerId;
        public int participantCount;
        public int prizePool;
    }

    [Serializable]
    public class AdvanceTournamentResponse
    {
        public string tournamentId;
        public int currentRound;
        public int remainingPlayers;
        public string status;
    }

    [Serializable]
    public class FinalizeTournamentResponse
    {
        public string tournamentId;
        public string firstPlaceId;
        public int firstPrize;
        public string secondPlaceId;
        public int secondPrize;
        public string thirdPlaceId;
        public int thirdPrize;
    }

    // ──────────────────────────────────────────────
    //  Request Bodies (for POST/PUT payloads)
    // ──────────────────────────────────────────────

    [Serializable]
    public class CreateDeckRequest
    {
        public string playerId;
        public List<string> adventurerCardIds;
        public List<string> enemyCardIds;
        public List<string> dungeonRoomIds;
        public string bossId;
    }

    [Serializable]
    public class UpdateDeckRequest
    {
        public List<string> adventurerCardIds;
        public List<string> enemyCardIds;
        public List<string> dungeonRoomIds;
        public string bossId;
    }

    [Serializable]
    public class CreateMatchRequest
    {
        public string player1Id;
        public string player2Id;
        public string player1DeckId;
        public string player2DeckId;
        public int startingHitPoints;
    }

    [Serializable]
    public class SetupInitialTeamRequest
    {
        public string playerId;
        public List<string> allyCardIds;
    }

    [Serializable]
    public class PlaceBetRequest
    {
        public string playerId;
        public int amount;
        public bool exile;
    }

    [Serializable]
    public class AssignCombatRequest
    {
        public string playerId;
        public List<CombatPairing> pairings;
    }

    [Serializable]
    public class RetargetRequest
    {
        public string playerId;
        public string allyId;
        public string newDefenderId;
        public int cost;
        public bool exileCost;
    }

    [Serializable]
    public class OpportunityAttackRequest
    {
        public string attackingPlayerId;
        public string attackerAllyId;
        public string fleeingAllyId;
    }

    [Serializable]
    public class ConcedeRoomRequest
    {
        public string defenderPlayerId;
    }

    [Serializable]
    public class OpenBoosterRequest
    {
        public string playerId;
        public int boosterPrice;
        public string setCode;
    }

    [Serializable]
    public class AddFundsRequest
    {
        public string playerId;
        public int amount;
        public string source;
    }

    [Serializable]
    public class ListCardRequest
    {
        public string sellerId;
        public string ownedCardId;
        public int price;
    }

    [Serializable]
    public class BuyCardRequest
    {
        public string buyerId;
    }

    [Serializable]
    public class JoinQueueRequest
    {
        public string playerId;
        public string deckId;
        public string queueType;
    }

    [Serializable]
    public class LeaveQueueRequest
    {
        public string playerId;
    }

    [Serializable]
    public class CreateTournamentRequest
    {
        public string requiredTier;
        public int entryFee;
    }

    [Serializable]
    public class JoinTournamentRequest
    {
        public string playerId;
        public string deckId;
    }

    [Serializable]
    public class AdvanceTournamentRequest
    {
        public string loserId;
    }

    [Serializable]
    public class UpdateEloRequest
    {
        public string winnerId;
        public string loserId;
    }
}
