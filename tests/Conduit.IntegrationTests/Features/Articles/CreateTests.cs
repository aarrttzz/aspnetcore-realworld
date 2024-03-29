using System.Linq;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Articles;
using Xunit;

namespace Conduit.IntegrationTests.Features.Articles;

public class CreateTests : SliceFixture
{
    [Fact]
    public async Task Expect_Create_Article()
    {
        var command = new CreateArticle.Command(
            new ArticleCreateDto
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
                TagList = new string[] { "tag1", "tag2" }
            }
        );

        var article = await ArticleHelpers.CreateArticle(this, command);

        Assert.NotNull(article);
        Assert.Equal(article.Title, command.Article.Title);
        Assert.Equal(article.TagList?.Count(), command.Article.TagList?.Count());
    }
}
