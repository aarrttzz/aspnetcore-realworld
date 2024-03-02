using System;
using System.Linq;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Articles;
using Conduit.Features.Comments;
using Conduit.IntegrationTests.Features.Comments;
using Conduit.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Conduit.IntegrationTests.Features.Articles;

public class DeleteTests : SliceFixture
{
    [Fact]
    public async Task Expect_Delete_Article()
    {
        var createCmd = new CreateArticle.Command(
            new ArticleCreateDto
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
            }
        );

        var article = await ArticleHelpers.CreateArticle(this, createCmd);
        var slug = article.Slug ?? throw new InvalidOperationException();

        var deleteCmd = new DeleteArticle.Command(slug);

        var dbContext = GetDbContext();

        var articleDeleteHandler = new DeleteArticle.Handler(dbContext);
        await articleDeleteHandler.Handle(deleteCmd, new System.Threading.CancellationToken());

        var dbArticle = await ExecuteDbContextAsync(
            db => db.Articles.Where(d => d.Slug == deleteCmd.Slug).SingleOrDefaultAsync()
        );

        Assert.Null(dbArticle);
    }

    [Fact]
    public async Task Expect_Delete_Article_With_Tags()
    {
        var createCmd = new CreateArticle.Command(
            new ArticleCreateDto
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
                TagList = new string[] { "tag1", "tag2" }
            }
        );

        var article = await ArticleHelpers.CreateArticle(this, createCmd);
        var dbArticleWithTags = await ExecuteDbContextAsync(
            db =>
                db.Articles
                    .Include(a => a.ArticleTags)
                    .Where(d => d.Slug == article.Slug)
                    .SingleOrDefaultAsync()
        );

        var deleteCmd = new DeleteArticle.Command(article.Slug?? throw new InvalidOperationException());

        var dbContext = GetDbContext();

        var articleDeleteHandler = new DeleteArticle.Handler(dbContext);
        await articleDeleteHandler.Handle(deleteCmd, new System.Threading.CancellationToken());

        var dbArticle = await ExecuteDbContextAsync(
            db => db.Articles.Where(d => d.Slug == deleteCmd.Slug).SingleOrDefaultAsync()
        );
        Assert.Null(dbArticle);
    }

    [Fact]
    public async Task Expect_Delete_Article_With_Comments()
    {
        var createArticleCmd = new CreateArticle.Command(
            new ArticleCreateDto
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
            }
        );

        var article = await ArticleHelpers.CreateArticle(this, createArticleCmd);
        var dbArticle = await ExecuteDbContextAsync(
            db =>
                db.Articles
                    .Include(a => a.ArticleTags)
                    .Where(d => d.Slug == article.Slug)
                    .SingleOrDefaultAsync()
        ) ?? throw new InvalidOperationException();

        var articleId = dbArticle.ArticleId;
        var slug = dbArticle.Slug;

        // create article comment
        var createCommentCommand = new CreateComment.Command(
            new CommentCreateDto
            {
                Body = "article comment"
            },
            slug?? throw new InvalidOperationException()
        );

        var comment = await CommentHelpers.CreateComment(
            this,
            createCommentCommand,
            UserHelpers.DefaultUserName
        );

        // delete article with comment
        var deleteCmd = new DeleteArticle.Command(slug);

        var dbContext = GetDbContext();

        var articleDeleteHandler = new DeleteArticle.Handler(dbContext);
        await articleDeleteHandler.Handle(deleteCmd, new System.Threading.CancellationToken());

        var deleted = await ExecuteDbContextAsync(
            db => db.Articles.Where(d => d.Slug == deleteCmd.Slug).SingleOrDefaultAsync()
        );
        Assert.Null(deleted);
    }
}
