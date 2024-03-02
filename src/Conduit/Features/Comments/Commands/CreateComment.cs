using System;
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

namespace Conduit.Features.Comments;

public class CreateComment
{
    public record Command(CommentCreateDto comment, string Slug) : IRequest<CommentDto>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.comment.Body).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, CommentDto>
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

        public async Task<CommentDto> Handle(
            Command message,
            CancellationToken cancellationToken
        )
        {
            var article = await _context.Articles
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

            if (article == null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );
            }

            var author = await _context.Persons.FirstAsync(
                x => x.Username == _currentUserAccessor.GetCurrentUsername(),
                cancellationToken
            );

            var comment = new Comment()
            {
                Author = author,
                Body = message.comment.Body ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.Comments.AddAsync(comment, cancellationToken);

            article.Comments.Add(comment);

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<Comment, CommentDto>(comment);
        }
    }
}
