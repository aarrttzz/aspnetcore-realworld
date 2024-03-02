using System.Threading;
using System.Threading.Tasks;
using Conduit.DTO;
using FluentValidation;
using MediatR;

namespace Conduit.Features.Profiles;

public class GetProfileDetails
{
    public record Query(string Username) : IRequest<ProfileDto>;

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Query, ProfileDto>
    {
        private readonly IProfileReader _profileReader;

        public Handler(IProfileReader profileReader)
        {
            _profileReader = profileReader;
        }

        public Task<ProfileDto> Handle(Query message, CancellationToken cancellationToken)
        {
            return _profileReader.ReadProfile(message.Username, cancellationToken);
        }
    }
}
