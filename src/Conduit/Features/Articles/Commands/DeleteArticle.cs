using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Articles;

public class DeleteArticle
{
    public record Command(string Slug) : IRequest;

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
                await _context.Articles.FirstOrDefaultAsync(
                    x => x.Slug == message.Slug,
                    cancellationToken
                )
                ?? throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync(cancellationToken);
            await Task.FromResult(Unit.Value);
        }
    }
}
