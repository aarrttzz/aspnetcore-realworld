using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Domain;
using Conduit.DTO;
using Conduit.Features.Articles;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Favorites;

public class AddArticleToFavorites
{
    public record Command(string Slug) : IRequest<ArticleDto>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Slug)).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, ArticleDto>
    {
        private readonly ConduitContext _context;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;

        public Handler(ConduitContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
        {
            _context = context;
            _currentUserAccessor = currentUserAccessor;
            _mapper = mapper;
        }

        public async Task<ArticleDto> Handle(
            Command message,
            CancellationToken cancellationToken
        )
        {
            var article = await _context.Articles.FirstOrDefaultAsync(
                x => x.Slug == message.Slug,
                cancellationToken
            );

            if (article == null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );
            }

            var person = await _context.Persons.FirstOrDefaultAsync(
                x => x.Username == _currentUserAccessor.GetCurrentUsername(),
                cancellationToken
            );

            if (person is null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );
            }

            var favorite = await _context.ArticleFavorites.FirstOrDefaultAsync(
                x => x.ArticleId == article.ArticleId && x.PersonId == person.PersonId,
                cancellationToken
            );

            if (favorite == null)
            {
                favorite = new ArticleFavorite()
                {
                    Article = article,
                    ArticleId = article.ArticleId,
                    Person = person,
                    PersonId = person.PersonId
                };
                await _context.ArticleFavorites.AddAsync(favorite, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }



            article =
                await _context.Articles
                    .GetAllData()
                    .FirstOrDefaultAsync(
                        x => x.ArticleId == article.ArticleId,
                        cancellationToken
                    );
            if (article is null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );
            }

            return _mapper.Map<Article, ArticleDto>(article);
        }
    }
}
