using System;
using System.Linq;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Articles;
using Xunit;

namespace Conduit.IntegrationTests.Features.Articles;

public class EditTests : SliceFixture
{
    [Fact]
    public async Task Expect_Edit_Article()
    {
        var createCommand = new CreateArticle.Command(
            new ArticleCreateDto
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
                TagList = new string[] { "tag1", "tag2" }
            }
        );

        var createdArticle = await ArticleHelpers.CreateArticle(this, createCommand);

        var command = new EditArticle.Command(
            new ArticleEditDto
            {
                Title = "Updated " + createdArticle.Title,
                Description = "Updated" + createdArticle.Description,
                Body = "Updated" + createdArticle.Body,
            },
            createdArticle.Slug ?? throw new InvalidOperationException()
        );
        // remove the first tag and add a new tag
        // TODO: 
        //command.Article.TagList = new string[] { createdArticle.TagList[1], "tag3" };

        var dbContext = GetDbContext();
        var mapper = GetMapper();

        var articleEditHandler = new EditArticle.Handler(dbContext, mapper);
        var edited = await articleEditHandler.Handle(
            command,
            new System.Threading.CancellationToken()
        );

        Assert.NotNull(edited);
        Assert.Equal(edited.Title, command.Article.Title);

        // TODO: uncomment after all modeles implementation
        //  Assert.Equal(edited.TagList.Count(), command.Article.TagList.Count());
        // use assert Contains because we do not know the order in which the tags are saved/retrieved
        // Assert.Contains(edited.Article.TagList[0], command.Model.Article.TagList);
        // Assert.Contains(edited.Article.TagList[1], command.Model.Article.TagList);
    }
}
