using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users;

[Route("user")]
[Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
public class UserController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpGet]
    public Task<UserDto> GetCurrent(CancellationToken cancellationToken)
    {
        var query = new GetUserByUsername.Query
            (_currentUserAccessor.GetCurrentUsername() ?? "<unknown>");
        return _mediator.Send(query, cancellationToken);
    }

    [HttpPut]
    public Task<UserDto> EditUser(
        [FromBody] UserEditDto user,
        CancellationToken cancellationToken
    )
    {
        var query = new EditUser.Command(user);
        return _mediator.Send(query, cancellationToken);
    }
}
