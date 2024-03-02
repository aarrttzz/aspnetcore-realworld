using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Profiles;

[Route("profiles")]
public class ProfilesController : Controller
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{username}")]
    public Task<ProfileDto> Get(string username, CancellationToken cancellationToken)
    {
        var query = new GetProfileDetails.Query(username);
        return _mediator.Send(query, cancellationToken);
    }
}
