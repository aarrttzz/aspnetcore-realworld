using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users;

[Route("users")]
public class UsersController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public Task<UserDto> Create(
        [FromBody] UserCreateDto user,
        CancellationToken cancellationToken
    )
    {
       var command = new CreateUser.Command(user);
       return _mediator.Send(command, cancellationToken);
    }

    [HttpPost("login")]
    public Task<UserDto> Login(
        [FromBody] UserLoginDto user,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginUser.Command(user);
        return _mediator.Send(command, cancellationToken);
    }
}
