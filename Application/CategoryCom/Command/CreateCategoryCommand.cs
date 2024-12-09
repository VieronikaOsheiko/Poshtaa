using Application.CategoryCom.CategoryException;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Category;
using MediatR;

namespace Application.CategoryCom.Command;

public record CreateCategoryCommand : IRequest<Result<Category,CategoryExceptions>> 
{
    public required string Name { get; init; }
    public required string Material { get; init; }
    public required bool InCountry { get; init; }
    public required string Size { get; init; }
}

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, Result<Category, CategoryExceptions>>
{
    public async Task<Result<Category, CategoryExceptions>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.SearchByName(request.Name, cancellationToken);
        return await existingCategory.Match(
            c => Task.FromResult<Result<Category,CategoryExceptions>> (new CategoryAlreadyExistsExceptions(c.Id)),
            async () => await CreateEntity(request.Name,request.Material,request.InCountry,request.Size,cancellationToken));
      
    }

    private async Task<Result<Category, CategoryExceptions>> CreateEntity(
        string name,
        string material,
        bool inCountry,
        string size,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var entity = Category.New(
                CategoryId.New(),
                name,
                inCountry,
                material,
                size
            );
            return await  categoryRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownExceptions(CategoryId.Empty, exception);
        }
    }

}