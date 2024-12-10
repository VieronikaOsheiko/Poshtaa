using Apii.Dtos;
using Apii.Modules.Errors;
using Application.CategoryCom;
using Application.Common.Interfaces.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Apii.Controllers;
[Route("category")]
[ApiController]
public class CategoryController(ISender sender, ICategoryQueries categoriesQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDtos>>> GetALL(CancellationToken cancellationToken)
    {
        var entities = await categoriesQueries.GetAll(cancellationToken);
        return entities.Select(CategoryDtos.FromDomainModel).ToList();
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryDtos>> Create(
        [FromBody] CategoryDtos request,
        CancellationToken cancellationToken)
    {
        var input = new CreateCategoryCommand
        {
            Name = request.Name,
            Size = request.Size,
            InCountry = request.InCountry,
            Material = request.Material,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDtos>>(
            f => CategoryDtos.FromDomainModel(f),
            e => e.ToObjectResult());
    }
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var input = new DeleteCategoryCommand
        {
            CategoryId = id
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());

    }
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<CategoryDtos>> Update(
        [FromBody] CategoryDtos request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryCommand
        {
            Name = request.Name,
            CategoryId = request.Id!.Value,
            Size = request.Size,
            InCountry = request.InCountry,
            Material = request.Material,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDtos>>(
            f => CategoryDtos.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}