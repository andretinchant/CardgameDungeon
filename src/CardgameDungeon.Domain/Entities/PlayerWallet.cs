using CardgameDungeon.Domain.Enums;

namespace CardgameDungeon.Domain.Entities;

public class PlayerWallet
{
    public const int DailyRewardLimit = 1;

    public Guid PlayerId { get; private set; }
    public int Balance { get; private set; }
    public DateOnly LastDailyRewardDate { get; private set; }
    public int DailyRewardsClaimedToday { get; private set; }

    public PlayerWallet(Guid playerId, int balance = 0)
    {
        PlayerId = playerId;
        Balance = balance;
        LastDailyRewardDate = DateOnly.MinValue;
        DailyRewardsClaimedToday = 0;
    }

    public void AddFunds(int amount, FundSource source, DateOnly today)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        if (source == FundSource.DailyReward)
        {
            ResetDailyCounterIfNewDay(today);

            if (DailyRewardsClaimedToday >= DailyRewardLimit)
                throw new InvalidOperationException("Daily reward limit already reached.");

            DailyRewardsClaimedToday++;
            LastDailyRewardDate = today;
        }

        Balance += amount;
    }

    public void Deduct(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        if (Balance < amount)
            throw new InvalidOperationException(
                $"Insufficient balance. Current: {Balance}, required: {amount}.");

        Balance -= amount;
    }

    private void ResetDailyCounterIfNewDay(DateOnly today)
    {
        if (today > LastDailyRewardDate)
            DailyRewardsClaimedToday = 0;
    }
}
