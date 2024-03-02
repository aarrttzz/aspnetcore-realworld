using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Comments;

public class DeleteComment
{
    public record Command(string Slug, int Id) : IRequest;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator() => RuleFor(x => x.Slug).NotNull().NotEmpty();
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ConduitContext _context;

        public Handler(ConduitContext context) => _context = context;

        public async Task Handle(Command message, CancellationToken cancellationToken)
        {
            var article =
                await _context.Articles
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken)
                ?? throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );

            var comment =
                article.Comments.FirstOrDefault(x => x.CommentId == message.Id)
                ?? throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Comment = Constants.NOT_FOUND }
                );

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
            await Task.FromResult(Unit.Value);
        }
    }
}
