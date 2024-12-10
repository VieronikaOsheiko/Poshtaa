using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Category;
using MediatR;

namespace Application.CategoryCom;

public class DeleteCategoryCommand: IRequest<Result<Category, CategoryException>>
{
    public required Guid CategoryId { get; init; }
}

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);

        var existingCourse = await categoryRepository.GetById(categoryId, cancellationToken);

        return await existingCourse.Match<Task<Result<Category, CategoryException>>>(
            async u => await DeleteEntity(u, cancellationToken),
            () => Task.FromResult<Result<Category,CategoryException>>(new CategoryNotFoundException(categoryId)));
    }

    public async Task<Result<Category, CategoryException>> DeleteEntity(Category category, CancellationToken cancellationToken)
    {
        try
        {
            return await categoryRepository.Delete(category, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownException(category.Id, exception);
        }
    }
}