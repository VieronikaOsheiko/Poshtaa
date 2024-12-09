using Application.CategoryCom.CategoryException;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Category;
using MediatR;

namespace Application.CategoryCom.Command;

public class DeleteCategoryCommand: IRequest<Result<Category, CategoryException.CategoryExceptions>>
{
    public required Guid CategoryId { get; init; }
}

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteCategoryCommand, Result<Category, CategoryException.CategoryExceptions>>
{
    public async Task<Result<Category, CategoryException.CategoryExceptions>> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);

        var existingCourse = await categoryRepository.GetById(categoryId, cancellationToken);

        return await existingCourse.Match<Task<Result<Category, CategoryException.CategoryExceptions>>>(
            async u => await DeleteEntity(u, cancellationToken),
            () => Task.FromResult<Result<Category,CategoryException.CategoryExceptions>>(new CategoryNotFoundExceptions(categoryId)));
    }

    public async Task<Result<Category, CategoryException.CategoryExceptions>> DeleteEntity(Category category, CancellationToken cancellationToken)
    {
        try
        {
            return await categoryRepository.Delete(category, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownExceptions(category.Id, exception);
        }
    }
}