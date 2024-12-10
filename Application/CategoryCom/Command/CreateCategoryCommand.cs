using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Category;
using MediatR;
namespace Application.CategoryCom;

public record CreateCategoryCommand : IRequest<Result<Category,CategoryException>> 
{
    public required string Name { get; init; }
    public required string Material { get; init; }
    public required bool InCountry { get; init; }
    public required string Size { get; init; }
}

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.SearchByName(request.Name, cancellationToken);
        return await existingCategory.Match(
            c => Task.FromResult<Result<Category,CategoryException>> (new CategoryAlreadyExistsException(c.Id)),
            async () => await CreateEntity(request.Name,request.Material,request.InCountry,request.Size,cancellationToken));
      
    }

    private async Task<Result<Category, CategoryException>> CreateEntity(
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
            return new CategoryUnknownException(CategoryId.Empty, exception);
        }
    }

}