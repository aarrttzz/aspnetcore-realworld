using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Domain;
using Conduit.DTO;
using Conduit.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Articles;

public class CreateArticle
{
    public record Command(ArticleCreateDto Article) : IRequest<ArticleDto>;

    public class ArticleDataValidator : AbstractValidator<ArticleCreateDto>
    {
        public ArticleDataValidator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty();
            RuleFor(x => x.Description).NotNull().NotEmpty();
            RuleFor(x => x.Body).NotNull().NotEmpty();
        }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Article).NotNull().SetValidator(new ArticleDataValidator());
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
            var author = await _context.Persons.FirstAsync(
                x => x.Username == _currentUserAccessor.GetCurrentUsername(),
                cancellationToken
            );
            var tags = new List<Tag>();
            foreach (var tag in (message.Article.TagList ?? Enumerable.Empty<string>()))
            {
                var t = await _context.Tags.FindAsync(tag);
                if (t == null)
                {
                    t = new Tag() { TagId = tag };
                    await _context.Tags.AddAsync(t, cancellationToken);
                    //save immediately for reuse
                    await _context.SaveChangesAsync(cancellationToken);
                }
                tags.Add(t);
            }

            var article = new Article()
            {
                Author = author,
                Body = message.Article.Body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Description = message.Article.Description,
                Title = message.Article.Title,
                Slug = message.Article.Title.GenerateSlug()
            };
            await _context.Articles.AddAsync(article, cancellationToken);

            await _context.ArticleTags.AddRangeAsync(
                tags.Select(x => new ArticleTag() { Article = article, Tag = x }),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);


            return _mapper.Map<Article, ArticleDto>(article);
        }
    }
}
