using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using MediatR;

namespace CardgameDungeon.Features.Collection.OpenBooster;

public class OpenBoosterHandler(
    ICollectionRepository collectionRepo,
    IWalletRepository walletRepo,
    IBoosterCardPool cardPool)
    : IRequestHandler<OpenBoosterCommand, OpenBoosterResponse>
{
    // Fixed booster composition: 1 Rare/Unique, 3 Uncommon, 6 Common
    private static readonly Rarity[] BoosterSlots =
    [
        Rarity.Rare,        // slot 1: Rare (could be Unique via pool logic)
        Rarity.Uncommon,    // slots 2-4
        Rarity.Uncommon,
        Rarity.Uncommon,
        Rarity.Common,      // slots 5-10
        Rarity.Common,
        Rarity.Common,
        Rarity.Common,
        Rarity.Common,
        Rarity.Common
    ];

    public async Task<OpenBoosterResponse> Handle(OpenBoosterCommand request, CancellationToken ct)
    {
        var setCode = string.IsNullOrWhiteSpace(request.SetCode)
            ? "DND1"
            : request.SetCode.Trim().ToUpperInvariant();

        var wallet = await walletRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Wallet for player {request.PlayerId} not found.");

        wallet.Deduct(request.BoosterPrice);

        var collection = await collectionRepo.GetByPlayerIdAsync(request.PlayerId, ct)
            ?? throw new KeyNotFoundException($"Collection for player {request.PlayerId} not found.");

        var result = new List<BoosterCardDto>();

        foreach (var rarity in BoosterSlots)
        {
            var card = await cardPool.GetRandomCardByRarityAsync(rarity, setCode, ct);
            collection.AddCard(card.Id);
            result.Add(new BoosterCardDto(card.Id, card.Name, card.Rarity, card.Type, setCode));
        }

        await walletRepo.UpdateAsync(wallet, ct);
        await collectionRepo.UpdateAsync(collection, ct);

        return new OpenBoosterResponse(request.PlayerId, setCode, result);
    }
}
