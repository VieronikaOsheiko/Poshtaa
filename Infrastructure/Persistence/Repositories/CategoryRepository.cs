using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Category;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace ClassLibrary1.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context): ICategoryRepository, ICategoryQueries
{
    public async Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Category>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Category>() : Option.Some(entity);
    }

    public async Task<Option<Category>> GetById(CategoryId id, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Category>() : Option.Some(entity);
    }

    

    public async Task<Category> Add(Category course, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(course, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return course;
    }

    public async Task<Category> Update(Category course, CancellationToken cancellationToken)
    {
        context.Categories.Update(course);

        await context.SaveChangesAsync(cancellationToken);

        return course;
    }
    public async Task<Category> Delete(Category course, CancellationToken cancellationToken)
    {
        context.Categories.Remove(course);
        await context.SaveChangesAsync(cancellationToken);
        return course;
    }
}