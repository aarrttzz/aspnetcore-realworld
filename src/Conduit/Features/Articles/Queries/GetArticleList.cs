using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Conduit.Domain;
using Conduit.DTO;

namespace Conduit.Features.Articles;

public class GetArticleList
{
    public record Query(ArticleListRequestDto article) : IRequest<ArticleListResponceDto>;

    public class QueryHandler : IRequestHandler<Query, ArticleListResponceDto>
    {
        private readonly ConduitContext _context;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;

        public QueryHandler(ConduitContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
        {
            _context = context;
            _currentUserAccessor = currentUserAccessor;
            _mapper = mapper;
        }

        public async Task<ArticleListResponceDto> Handle(
            Query message,
            CancellationToken cancellationToken
        )
        {
            var queryable = _context.Articles.GetAllData();

            if (message.article.IsFeed && _currentUserAccessor.GetCurrentUsername() != null)
            {
                var currentUser = await _context.Persons
                    .Include(x => x.Following)
                    .FirstOrDefaultAsync(
                        x => x.Username == _currentUserAccessor.GetCurrentUsername(),
                        cancellationToken
                    );

                if (currentUser is null)
                {
                    throw new RestException(
                        HttpStatusCode.NotFound,
                        new { User = Constants.NOT_FOUND }
                    );
                }
                queryable = queryable.Where(
                    x =>
                        currentUser.Following
                            .Select(y => y.TargetId)
                            .Contains(x.Author!.PersonId)
                );
            }

            if (!string.IsNullOrWhiteSpace(message.article.Tag))
            {
                var tag = await _context.ArticleTags.FirstOrDefaultAsync(
                    x => x.TagId == message.article.Tag,
                    cancellationToken
                );
                if (tag != null)
                {
                    queryable = queryable.Where(
                        x => x.ArticleTags.Select(y => y.TagId).Contains(tag.TagId)
                    );
                }
                else
                {
                    return new ArticleListResponceDto();
                }
            }

            if (!string.IsNullOrWhiteSpace(message.article.Author))
            {
                var author = await _context.Persons.FirstOrDefaultAsync(
                    x => x.Username == message.article.Author,
                    cancellationToken
                );
                if (author != null)
                {
                    queryable = queryable.Where(x => x.Author == author);
                }
                else
                {
                    return new ArticleListResponceDto();
                }
            }

            if (!string.IsNullOrWhiteSpace(message.article.FavoritedUsername))
            {
                var author = await _context.Persons.FirstOrDefaultAsync(
                    x => x.Username == message.article.FavoritedUsername,
                    cancellationToken
                );
                if (author != null)
                {
                    queryable = queryable.Where(
                        x => x.ArticleFavorites.Any(y => y.PersonId == author.PersonId)
                    );
                }
                else
                {
                    return new ArticleListResponceDto();
                }
            }

            var articles = await queryable
                .OrderByDescending(x => x.CreatedAt)
                .Skip(message.article.Offset ?? 0)
                .Take(message.article.Limit ?? 20)
                .AsNoTracking()
            .ToListAsync(cancellationToken);




            return new ArticleListResponceDto()
            {
                Articles = _mapper.Map<List<Article>, List<ArticleDto>>(articles),
                ArticlesCount = queryable.Count()
            };
        }
    }
}
