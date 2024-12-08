using Domain.Category;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Option<Category>> GetById(CategoryId id, CancellationToken cancellationToken);
    Task<Option<Category>> SearchByName(string name,CancellationToken cancellationToken);
    Task<Category> Update(Category course, CancellationToken cancellationToken);
    Task<Category> Add(Category course, CancellationToken cancellationToken);
    Task<Category> Delete(Category course, CancellationToken cancellationToken);
}