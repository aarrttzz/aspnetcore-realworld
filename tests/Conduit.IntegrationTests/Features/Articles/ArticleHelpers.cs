using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.DTO;
using Conduit.Features.Articles;
using Conduit.Infrastructure.Errors;
using Conduit.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace Conduit.IntegrationTests.Features.Articles;

public static class ArticleHelpers
{
    /// <summary>
    /// creates an article based on the given Create command. It also creates a default user
    /// </summary>
    /// <param name="fixture"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    public static async Task<ArticleDto> CreateArticle(
        SliceFixture fixture,
        CreateArticle.Command command
    )
    {
        // first create the default user
        var user = await UserHelpers.CreateDefaultUser(fixture);
        if (user.Username is null)
        {
            throw new RestException(HttpStatusCode.BadRequest);
        }

        var dbContext = fixture.GetDbContext();
        var currentAccessor = new StubCurrentUserAccessor(user.Username);
        var mapper = fixture.GetMapper();

        var articleCreateHandler = new CreateArticle.Handler(dbContext, currentAccessor, mapper);
        var created = await articleCreateHandler.Handle(
            command,
            new System.Threading.CancellationToken()
        );

        var dbArticle = await fixture.ExecuteDbContextAsync(
            db =>
                db.Articles
                    .Where(a => a.Slug == created.Slug)
                    .SingleOrDefaultAsync()
        );
        if (dbArticle is null)
        {
            throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NOT_FOUND});

        }

        return created;
    }
}
