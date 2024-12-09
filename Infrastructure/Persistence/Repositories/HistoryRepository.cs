using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.History;
using Domain.Parcels;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace ClassLibrary1.Persistence.Repositories;

public class HistoryRepository : IHistoryRepository, IHistoryQueries
{
    private readonly ApplicationDbContext _context;

    public HistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<History>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Histories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<History>> GetById(HistoryId id, CancellationToken cancellationToken)
    {
        var entity = await _context.Histories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<History>() : Option.Some(entity);
    }

    public async Task<Option<History>> GetByParcelIdAndUserId(ParcelId parcelId, UserId userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Histories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ParcelId == parcelId && x.UserId == userId, cancellationToken);

        return entity == null ? Option.None<History>() : Option.Some(entity);
    }

    public async Task<List<History>> GetListParcelId(ParcelId parcelId, CancellationToken cancellationToken)
    {
        return await _context.Histories
            .AsNoTracking()
            .Where(x => x.ParcelId == parcelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<History> Add(History courseUser, CancellationToken cancellationToken)
    {
        await _context.Histories.AddAsync(courseUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return courseUser;
    }

    public async Task<History> Update(History courseUser, CancellationToken cancellationToken)
    {
        _context.Histories.Update(courseUser);
        await _context.SaveChangesAsync(cancellationToken);
        return courseUser;
    }

    public async Task<History> Delete(History courseUser, CancellationToken cancellationToken)
    {
        _context.Histories.Remove(courseUser);
        await _context.SaveChangesAsync(cancellationToken);
        return courseUser;
    }
}
