using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Profiles;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Followers;

[Route("profiles")]
public class FollowersController : Controller
{
    private readonly IMediator _mediator;

    public FollowersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{username}/follow")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ProfileDto> Follow(string username, CancellationToken cancellationToken)
    {
        var command = new AddUserToFollowers.Command(username);
        return _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{username}/follow")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ProfileDto> Unfollow(string username, CancellationToken cancellationToken)
    {
        var command = new DeleteUserFromFollowers.Command(username);
        return _mediator.Send(command, cancellationToken);
    }
}
