using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Parcels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace ClassLibrary1.Persistence.Repositories;

public class ParcelRepository(ApplicationDbContext context) : IParcelRepository, IParcelQueries
{

    public async Task<Option<Parcel>> GetByWeightAndAdrres(
        string weight,
        string addresToCome,
        CancellationToken cancellationToken)
    {
        var entity = await context.Parcels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Weight == weight && x.AddresToCome == addresToCome, cancellationToken);

        return entity == null ? Option.None<Parcel>() : Option.Some(entity);
    }
    public async Task<IReadOnlyList<Parcel>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Parcels
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Parcel>> GetById(ParcelId id, CancellationToken cancellationToken)
    {
        var entity = await context.Parcels
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Parcel>() : Option.Some(entity);
    }

    public async Task<Parcel> Add(Parcel user, CancellationToken cancellationToken)
    {
        await context.Parcels.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }
    public async Task<Parcel> Update(Parcel user, CancellationToken cancellationToken)
    {
        context.Parcels.Update(user);

        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<Parcel> Delete(Parcel user, CancellationToken cancellationToken)
    {
        context.Parcels.Remove(user);

        await context.SaveChangesAsync(cancellationToken);

        return user;
    }
}