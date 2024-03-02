using System.Threading;
using System.Threading.Tasks;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Conduit.DTO;
using Conduit.Domain;

namespace Conduit.Features.Articles;


[Route("articles")]
public class ArticlesController : Controller
{
    private readonly IMediator _mediator;

    public ArticlesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public Task<ArticleListResponceDto> Get(
        [FromQuery] string tag,
        [FromQuery] string author,
        [FromQuery] string favorited,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        CancellationToken cancellationToken
    )
    {
        var requestDto = new ArticleListRequestDto(tag, author, favorited, limit, offset);
        var query = new GetArticleList.Query(requestDto);
        return _mediator.Send(query, cancellationToken);
    }

    [HttpGet("feed")]
    public Task<ArticleListResponceDto> GetFeed(
        [FromQuery] string tag,
        [FromQuery] string author,
        [FromQuery] string favorited,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        CancellationToken cancellationToken
    )
    {
        var requestDto = new ArticleListRequestDto(tag, author, favorited, limit, offset) { IsFeed = true };
        var query = new GetArticleList.Query(requestDto);
        return _mediator.Send(query, cancellationToken);
    }

    [HttpGet("{slug}")]
    public Task<ArticleDto> Get(string slug, CancellationToken cancellationToken)
    {
        var query = new GetArticleBySlug.Query(slug);
        return _mediator.Send(query, cancellationToken);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ArticleDto> Create(
        [FromBody] ArticleCreateDto article,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateArticle.Command(article);
        return _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{slug}")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ArticleDto> Edit(
        string slug,
        [FromBody] ArticleEditDto article,
        CancellationToken cancellationToken
    )
    {
        var command = new EditArticle.Command(article, slug);
        return _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{slug}")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task Delete(string slug, CancellationToken cancellationToken)
    {
        var command = new DeleteArticle.Command(slug);
        return _mediator.Send(command, cancellationToken);
    }
}
