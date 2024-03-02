using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Users;

namespace Conduit.IntegrationTests.Features.Users;

public static class UserHelpers
{
    public static readonly string DefaultUserName = "username";

    /// <summary>
    /// creates a default user to be used in different tests
    /// </summary>
    /// <param name="fixture"></param>
    /// <returns></returns>
    public static async Task<UserDto> CreateDefaultUser(SliceFixture fixture)
    {
        var command = new CreateUser.Command(
            new UserCreateDto
            {
                Email = "email",
                Password = "password",
                Username = DefaultUserName
            }
        );

        var commandResult = await fixture.SendAsync(command);
        return commandResult;
    }
}
