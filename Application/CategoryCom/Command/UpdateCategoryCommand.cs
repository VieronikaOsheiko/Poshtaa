using Application.CategoryCom.CategoryException;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Category;
using MediatR;
using Optional;

namespace Application.CategoryCom.Command;

public record UpdateCategoryCommand: IRequest<Result<Category, CategoryException.CategoryExceptions>>
{
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Material { get; init; }
    public required bool InCountry { get; init; }
    public required string Size { get; init; }
}
public class UpdateCourseCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<Category, CategoryException.CategoryExceptions>>
{
    private readonly ICategoryRepository categoryRepository;

    public UpdateCourseCommandHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<Result<Category, CategoryException.CategoryExceptions>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);
        var course = await categoryRepository.GetById(categoryId, cancellationToken);

        return await course.Match(
            async f =>
            {
                var existingFaculty = await CheckDuplicated(categoryId, request.Name, cancellationToken);

                return await existingFaculty.Match(
                    ef => Task.FromResult<Result<Category, CategoryException.CategoryExceptions>>(new CategoryAlreadyExistsExceptions(ef.Id)),
                    async () => await UpdateEntity(f, request.Name, request.InCountry, request.Size, request.Material, cancellationToken));
            },
            () => Task.FromResult<Result<Category, CategoryException.CategoryExceptions>>(new CategoryNotFoundExceptions(categoryId)));
    }

    private async Task<Result<Category, CategoryException.CategoryExceptions>> UpdateEntity(
        Category course,
        string name,
        bool inCountry,  
        string sizer,
        string material,
        CancellationToken cancellationToken)
    {
        try
        {
            course.UpdateDetails(name, inCountry, material, sizer); 

            return await categoryRepository.Update(course, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownExceptions(course.Id, exception);
        }
    }

    private async Task<Option<Category>> CheckDuplicated(
        CategoryId courseId,
        string name,
        CancellationToken cancellationToken)
    {
        var course = await categoryRepository.SearchByName(name, cancellationToken);

        return course.Match(
            f => f.Id == courseId ? Option.None<Category>() : Option.Some(f), 
            Option.None<Category>);
    }
}
