using Apii.Dtos;
using Apii.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Apii.Controllers;
[Route("parcel")]
[ApiController]
public class ParcelController(ISender sender, IParcelQueries parcelQueries) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ParcelDtos>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await parcelQueries.GetAll(cancellationToken);
        return entities.Select(ParcelDtos.FromDomainModel).ToList();
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ParcelDtos>> Create([FromBody] ParcelDtos request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        var input = new CreateParcelCommand
        {
            ParcelId = Guid.NewGuid(),
            Weight = request.Weight,
            AddresToCome = request.AddresToCome,
            DateOfShipment = request.DateOfShipment,
            TrackNumber = request.TrackNumber,
            ReceiverId = request.ReceiverId,
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            UserIdFromToken = userIdFromToken
            
        };
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ParcelDtos>>(
            u => ParcelDtos.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ParcelDtos>> Update([FromBody] ParcelDtos request, CancellationToken cancellationToken)
    {
        var input = new UpdateParcelCommand()
        {
            ParcelId = request.Id!.Value,
            Weight = request.Weight,
            TrackNumber = request.TrackNumber,
            DateOfShipment = request.DateOfShipment,
            AddresToCome = request.AddresToCome,
            UserId = request.UserId,        
            ReceiverId = request.ReceiverId,
            CategoryId = request.CategoryId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ParcelDtos>>(
            parcel => ParcelDtos.FromDomainModel(parcel),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpDelete("{parcelId:guid}")]
    public async Task<ActionResult<ParcelDtos>> Delete([FromRoute] Guid parcelId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userId = new Guid(userIdClaim!.Value);
        var input = new DeleteParcelCommand
        {
            ParcelId = parcelId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ParcelDtos>>(
            u => ParcelDtos.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}