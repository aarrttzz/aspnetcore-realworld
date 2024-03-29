using System;
using System.Threading.Tasks;
using Conduit.Domain;
using Conduit.DTO;
using Conduit.Features.Users;
using Conduit.Infrastructure.Security;
using Xunit;

namespace Conduit.IntegrationTests.Features.Users;

public class LoginTests : SliceFixture
{
    [Fact]
    public async Task Expect_Login()
    {
        var salt = Guid.NewGuid().ToByteArray();
        var person = new Person
        {
            Username = "username",
            Email = "email",
            Hash = await new PasswordHasher().Hash("password", salt),
            Salt = salt
        };
        await InsertAsync(person);

        var command = new LoginUser.Command(
            new UserLoginDto { Email = "email", Password = "password" }
        );

        var user = await SendAsync(command);

        Assert.NotNull(user);
        Assert.Equal(user.Email, command.User.Email);
        Assert.Equal("username", user.Username);
        Assert.NotNull(user.Token);
    }
}
