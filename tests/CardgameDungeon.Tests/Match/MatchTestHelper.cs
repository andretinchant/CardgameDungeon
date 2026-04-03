using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Tests.Deck;
using CardgameDungeon.Tests.Deck.Fakes;

namespace CardgameDungeon.Tests.Match;

public static class MatchTestHelper
{
    public static AllyCard MakeAlly(
        int strength = 3, int hp = 5, int initiative = 2,
        bool isAmbusher = false, int cost = 1, string? name = null)
        => new(Guid.NewGuid(), name ?? $"Ally-{Guid.NewGuid():N}"[..12],
            Rarity.Common, cost, strength, hp, initiative, isAmbusher);

    public static DungeonRoomCard MakeRoom(int order, bool hasMonsters = true)
    {
        var monsterIds = hasMonsters ? new[] { Guid.NewGuid() } : null;
        return new DungeonRoomCard(Guid.NewGuid(), $"Room-{order}", Rarity.Common, order, monsterIds);
    }

    public static BossCard MakeBoss()
        => new(Guid.NewGuid(), "TheBoss", Rarity.Unique, 1, 10, 20, 5);

    /// <summary>
    /// Creates a match in Setup phase with both players having allies in hand.
    /// All allies have cost=1 so choosing 5 = exact cost of 5.
    /// </summary>
    public static MatchState MakeMatchInSetup(
        Guid? player1Id = null,
        Guid? player2Id = null,
        int allyCount = 20)
    {
        var p1Id = player1Id ?? Guid.NewGuid();
        var p2Id = player2Id ?? Guid.NewGuid();

        var p1Cards = Enumerable.Range(0, allyCount).Select(_ => (Card)MakeAlly()).ToList();
        var p2Cards = Enumerable.Range(0, allyCount).Select(_ => (Card)MakeAlly()).ToList();

        var player1 = new PlayerState(p1Id, 20, p1Cards);
        var player2 = new PlayerState(p2Id, 20, p2Cards);

        player1.RefillHand();
        player2.RefillHand();

        var rooms = Enumerable.Range(1, 5).Select(i => MakeRoom(i)).ToList();
        var boss = MakeBoss();

        return new MatchState(Guid.NewGuid(), player1, player2, rooms, boss);
    }

    /// <summary>
    /// Creates a match in Initiative phase (both teams already revealed).
    /// </summary>
    public static MatchState MakeMatchInInitiative(
        int p1Initiative = 3, int p2Initiative = 3,
        Guid? player1Id = null, Guid? player2Id = null)
    {
        var p1Id = player1Id ?? Guid.NewGuid();
        var p2Id = player2Id ?? Guid.NewGuid();

        // Create allies with specific initiative values
        var p1Allies = Enumerable.Range(0, 8)
            .Select(i => i < 5
                ? (Card)MakeAlly(initiative: p1Initiative, cost: 1)
                : (Card)MakeAlly())
            .ToList();
        var p2Allies = Enumerable.Range(0, 8)
            .Select(i => i < 5
                ? (Card)MakeAlly(initiative: p2Initiative, cost: 1)
                : (Card)MakeAlly())
            .ToList();

        // Add extra deck cards for bets
        var p1Deck = p1Allies.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();
        var p2Deck = p2Allies.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();

        var player1 = new PlayerState(p1Id, 20, p1Deck);
        var player2 = new PlayerState(p2Id, 20, p2Deck);

        player1.RefillHand();
        player2.RefillHand();

        var rooms = Enumerable.Range(1, 5).Select(i => MakeRoom(i)).ToList();
        var boss = MakeBoss();

        var match = new MatchState(Guid.NewGuid(), player1, player2, rooms, boss);

        // Submit teams with cost=5 (5 allies x cost 1)
        var p1Team = player1.Hand.OfType<AllyCard>().Take(5).ToList();
        var p2Team = player2.Hand.OfType<AllyCard>().Take(5).ToList();
        match.SubmitSetupTeam(p1Id, p1Team);
        match.SubmitSetupTeam(p2Id, p2Team);
        match.RevealTeams();
        match.RevealRoom();

        return match;
    }

    /// <summary>
    /// Creates a match in Combat phase with initiative resolved.
    /// P1 wins initiative by default. Both players have 5 allies in play.
    /// </summary>
    public static MatchState MakeMatchInCombat(
        int p1Strength = 3, int p1Hp = 5,
        int p2Strength = 3, int p2Hp = 5,
        Guid? player1Id = null, Guid? player2Id = null)
    {
        var p1Id = player1Id ?? Guid.NewGuid();
        var p2Id = player2Id ?? Guid.NewGuid();

        // P1 gets higher initiative to win
        var p1Allies = Enumerable.Range(0, 8)
            .Select(i => i < 5
                ? (Card)MakeAlly(strength: p1Strength, hp: p1Hp, initiative: 5, cost: 1)
                : (Card)MakeAlly())
            .ToList();
        var p2Allies = Enumerable.Range(0, 8)
            .Select(i => i < 5
                ? (Card)MakeAlly(strength: p2Strength, hp: p2Hp, initiative: 2, cost: 1)
                : (Card)MakeAlly())
            .ToList();

        var p1Deck = p1Allies.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();
        var p2Deck = p2Allies.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();

        var player1 = new PlayerState(p1Id, 20, p1Deck);
        var player2 = new PlayerState(p2Id, 20, p2Deck);

        player1.RefillHand();
        player2.RefillHand();

        var rooms = Enumerable.Range(1, 5).Select(i => MakeRoom(i)).ToList();
        var boss = MakeBoss();

        var match = new MatchState(Guid.NewGuid(), player1, player2, rooms, boss);

        var p1Team = player1.Hand.OfType<AllyCard>().Take(5).ToList();
        var p2Team = player2.Hand.OfType<AllyCard>().Take(5).ToList();
        match.SubmitSetupTeam(p1Id, p1Team);
        match.SubmitSetupTeam(p2Id, p2Team);
        match.RevealTeams();
        match.RevealRoom();

        // Resolve initiative — P1 wins (initiative 5 > 2)
        match.ResolveInitiative(
            player1.AlliesInPlay.Sum(a => a.Initiative),
            player2.AlliesInPlay.Sum(a => a.Initiative));

        // Winner chooses to attack
        match.ChooseRole(match.InitiativeWinnerId!.Value, choosesToAttack: true);

        return match;
    }

    /// <summary>
    /// Creates a match in Combat phase with specific ambusher configuration.
    /// </summary>
    public static MatchState MakeMatchInCombatWithAmbusher(
        Guid? player1Id = null, Guid? player2Id = null)
    {
        var p1Id = player1Id ?? Guid.NewGuid();
        var p2Id = player2Id ?? Guid.NewGuid();

        // P1 allies: normal, P2 allies: 4 normal + 1 ambusher
        var p1Cards = Enumerable.Range(0, 8)
            .Select(i => i < 5
                ? (Card)MakeAlly(initiative: 5, cost: 1)
                : (Card)MakeAlly())
            .ToList();

        var p2Cards = new List<Card>();
        for (var i = 0; i < 4; i++)
            p2Cards.Add(MakeAlly(initiative: 2, cost: 1, isAmbusher: false, name: $"Normal-{i}"));
        p2Cards.Add(MakeAlly(initiative: 2, cost: 1, isAmbusher: true, name: "Ambusher"));
        for (var i = 0; i < 3; i++)
            p2Cards.Add(MakeAlly());

        var p1Deck = p1Cards.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();
        var p2Deck = p2Cards.Concat(Enumerable.Range(0, 20).Select(_ => (Card)MakeAlly())).ToList();

        var player1 = new PlayerState(p1Id, 20, p1Deck);
        var player2 = new PlayerState(p2Id, 20, p2Deck);

        player1.RefillHand();
        player2.RefillHand();

        var rooms = Enumerable.Range(1, 5).Select(i => MakeRoom(i)).ToList();
        var boss = MakeBoss();

        var match = new MatchState(Guid.NewGuid(), player1, player2, rooms, boss);

        var p1Team = player1.Hand.OfType<AllyCard>().Take(5).ToList();
        var p2Team = player2.Hand.OfType<AllyCard>().Take(5).ToList();
        match.SubmitSetupTeam(p1Id, p1Team);
        match.SubmitSetupTeam(p2Id, p2Team);
        match.RevealTeams();
        match.RevealRoom();

        match.ResolveInitiative(
            player1.AlliesInPlay.Sum(a => a.Initiative),
            player2.AlliesInPlay.Sum(a => a.Initiative));

        // Winner chooses to attack
        match.ChooseRole(match.InitiativeWinnerId!.Value, choosesToAttack: true);

        return match;
    }

    public static DeckList MakeValidDeckForPlayer(Guid playerId)
    {
        var (adventurers, enemies, rooms, boss) = DeckTestHelper.MakeFullDeckCards();
        return new DeckList(Guid.NewGuid(), playerId, adventurers, enemies, rooms, boss);
    }
}
