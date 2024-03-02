using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Domain;
using Conduit.DTO;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Articles;

public class GetArticleBySlug
{
    public record Query(string Slug) : IRequest<ArticleDto>;

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator() => RuleFor(x => x.Slug).NotNull().NotEmpty();
    }

    public class QueryHandler : IRequestHandler<Query, ArticleDto>
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public QueryHandler(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ArticleDto> Handle(
            Query message,
            CancellationToken cancellationToken
        )
        {
            var article = await _context.Articles
                .GetAllData()
                .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

            if (article == null)
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
