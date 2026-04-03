using System.Text.Json;
using CardgameDungeon.Domain.Entities;
using CardgameDungeon.Domain.Enums;
using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Infrastructure.Data;
using CardgameDungeon.Infrastructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardgameDungeon.Infrastructure.Repositories;

public class EfMatchRepository(CardgameDungeonDbContext db) : IMatchRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<MatchState?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.MatchStates.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (entity is null) return null;

        var dto = JsonSerializer.Deserialize<MatchStateDto>(entity.StateJson, JsonOptions)!;
        return MatchStateMapper.FromDto(dto);
    }

    public async Task SaveAsync(MatchState match, CancellationToken ct = default)
    {
        var entity = ToEntity(match);
        db.MatchStates.Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MatchState match, CancellationToken ct = default)
    {
        var entity = await db.MatchStates.FirstOrDefaultAsync(m => m.Id == match.Id, ct);
        if (entity is null)
        {
            entity = ToEntity(match);
            db.MatchStates.Add(entity);
        }
        else
        {
            entity.Phase = (int)match.Phase;
            entity.WinnerId = match.WinnerId;
            entity.StateJson = SerializeState(match);
        }
        await db.SaveChangesAsync(ct);
    }

    private static MatchStateEntity ToEntity(MatchState match) => new()
    {
        Id = match.Id,
        Phase = (int)match.Phase,
        WinnerId = match.WinnerId,
        StateJson = SerializeState(match)
    };

    private static string SerializeState(MatchState match)
    {
        var dto = MatchStateMapper.ToDto(match);
        return JsonSerializer.Serialize(dto, JsonOptions);
    }
}
