using Apii.Dtos;
using Apii.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Historyy.Command;
using Domain.History;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Apii.Controllers
{
    [Route("Histories")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IHistoryQueries _historyQueriesQueries;

        public HistoryController(ISender sender, IHistoryQueries historyQueries)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _historyQueriesQueries = historyQueries ?? throw new ArgumentNullException(nameof(historyQueries));
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<HistoryDtos>>> GetAll(CancellationToken cancellationToken)
        {
            var entities = await _historyQueriesQueries.GetAll(cancellationToken);
            return Ok(entities.Select(HistoryDtos.FromDomainModel).ToList());
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<HistoryDtos>> Create(
            [FromBody] HistoryDtos request,
            CancellationToken cancellationToken)
        {
            var input = new CreateHistoryCommand
            {
                ParcelId = request.ParcelId,
                UserId = request.UserId,
                DataReceived = request.DataReceived,
            };

            var result = await _sender.Send(input, cancellationToken);

            return result.Match<ActionResult<HistoryDtos>>(
                f => HistoryDtos.FromDomainModel(f),
                e => e.ToObjectResult());
        }
        [Authorize]
        [HttpDelete("{historyId:guid}")]
        public async Task<ActionResult<HistoryDtos>> Delete([FromRoute] Guid historyId, CancellationToken cancellationToken)
        {
            var input = new DeleteHistoryCommand
            {
                HistoryId = historyId
            };

            var result = await _sender.Send(input, cancellationToken);

            return result.Match<ActionResult<HistoryDtos>>(
                u => Ok(HistoryDtos.FromDomainModel(u)),
                e => e.ToObjectResult());
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<HistoryDtos>> Update(
            [FromBody] HistoryDtos request,
            CancellationToken cancellationToken)
        {
            
            var input = new UpdateHistoryCommand
            {
                Id = request.Id.Value,
                ParcelId = request.ParcelId,
                UserId = request.UserId,
                DataRecived = request.DataReceived,
            };

            var result = await _sender.Send(input, cancellationToken);

            return result.Match<ActionResult<HistoryDtos>>(
                f => HistoryDtos.FromDomainModel(f),
                e => e.ToObjectResult());
        }
    }
}
