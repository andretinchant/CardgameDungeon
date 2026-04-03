using CardgameDungeon.Domain.Entities;

namespace CardgameDungeon.Domain.Repositories;

public interface IWalletRepository
{
    Task<PlayerWallet?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task SaveAsync(PlayerWallet wallet, CancellationToken ct = default);
    Task UpdateAsync(PlayerWallet wallet, CancellationToken ct = default);
}
