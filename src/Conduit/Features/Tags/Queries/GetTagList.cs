using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using Conduit.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Tags;

public class GetTagList
{
    public record Query : IRequest<TagListResponceDto>;

    public class QueryHandler : IRequestHandler<Query, TagListResponceDto>
    {
        private readonly ConduitContext _context;

        public QueryHandler(ConduitContext context)
        {
            _context = context;
        }

        public async Task<TagListResponceDto> Handle(
            Query message,
            CancellationToken cancellationToken
        )
        {
            var tags = await _context.Tags
                .OrderBy(x => x.TagId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return new TagListResponceDto()
            {
                Tags = tags?.Select(x => x.TagId ?? string.Empty).ToArray() ?? Array.Empty<string>()
            };
        }
    }
}
