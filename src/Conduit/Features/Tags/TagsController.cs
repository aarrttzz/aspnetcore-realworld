using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Tags;

[Route("tags")]
public class TagsController : Controller
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public Task<TagListResponceDto> Get(CancellationToken cancellationToken)
    {
        var query = new GetTagList.Query();
        return _mediator.Send(query, cancellationToken);
    }
}
