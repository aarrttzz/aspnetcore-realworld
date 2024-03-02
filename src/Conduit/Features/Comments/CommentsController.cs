using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Comments;

[Route("articles")]
public class CommentsController : Controller
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{slug}/comments")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<CommentDto> Create(
        string slug,
        [FromBody] CommentCreateDto comment,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateComment.Command(comment, slug);
        return _mediator.Send(command, cancellationToken);
    }

    [HttpGet("{slug}/comments")]
    public Task<List<CommentDto>> Get(string slug, CancellationToken cancellationToken)
    {
        var query = new GetCommentList.Query(slug);
        return _mediator.Send(query, cancellationToken);
    }

    [HttpDelete("{slug}/comments/{id}")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task Delete(string slug, int id, CancellationToken cancellationToken)
    {
        var command = new DeleteComment.Command(slug, id);
        return _mediator.Send(command, cancellationToken);
    }
}
