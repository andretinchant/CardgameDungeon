using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Repositories;

namespace CardgameDungeon.Tests.MetaSystems.Fakes;

public class FakeWalletRepository : IWalletRepository
{
    private readonly Dictionary<Guid, PlayerWallet> _wallets = new();

    public Task<PlayerWallet?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_wallets.GetValueOrDefault(playerId));

    public Task SaveAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        _wallets[wallet.PlayerId] = wallet;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlayerWallet wallet, CancellationToken ct = default)
    {
        _wallets[wallet.PlayerId] = wallet;
        return Task.CompletedTask;
    }

    public void Seed(PlayerWallet wallet) => _wallets[wallet.PlayerId] = wallet;
}
