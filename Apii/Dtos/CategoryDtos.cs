using Domain.Category;

namespace Apii.Dtos;

public record CategoryDtos
(
    
    Guid? Id,
    string Name,
    string Material,
    bool InCountry,
    string Size
)
{
    public static CategoryDtos FromDomainModel(Category category)
        => new
        (
            category.Id.Value,
            category.Name,
            category.Material,
            category.InCountry,
            category.Size

        );
}