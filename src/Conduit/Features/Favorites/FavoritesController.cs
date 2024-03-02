using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Features.Articles;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Favorites;

[Route("articles")]
public class FavoritesController : Controller
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{slug}/favorite")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ArticleDto> FavoriteAdd(string slug, CancellationToken cancellationToken)
    {
        var command = new AddArticleToFavorites.Command(slug);
        return _mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{slug}/favorite")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public Task<ArticleDto> FavoriteDelete(
        string slug,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteArticleFromFavorites.Command(slug);
        return _mediator.Send(command, cancellationToken);
    }
}
