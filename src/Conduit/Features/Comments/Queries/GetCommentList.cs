using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Domain;
using Conduit.DTO;  
using Conduit.Features.Tags;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Comments;

public class GetCommentList
{
    public record Query(string Slug) : IRequest<List<CommentDto>>;

    public class Handler : IRequestHandler<Query, List<CommentDto>>
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public Handler(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> Handle(
            Query message,
            CancellationToken cancellationToken
        )
        {
            var article = await _context.Articles
                .Include(x => x.Comments)
                .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

            if (article == null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { Article = Constants.NOT_FOUND }
                );
            }

            return _mapper.Map<List<Comment>, List<CommentDto>>(article.Comments);
        }
    }
}
