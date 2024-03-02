using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.DTO;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using Conduit.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Users;

public class EditUser
{
    public record Command(UserEditDto User) : IRequest<UserDto>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.User).NotNull();
        }
    }

    public class Handler : IRequestHandler<Command, UserDto>
    {
        private readonly ConduitContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;

        public Handler(
            ConduitContext context,
            IPasswordHasher passwordHasher,
            ICurrentUserAccessor currentUserAccessor,
            IMapper mapper
        )
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _currentUserAccessor = currentUserAccessor;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(
            Command message,
            CancellationToken cancellationToken
        )
        {
            var currentUsername = _currentUserAccessor.GetCurrentUsername();
            var person = await _context.Persons
                .Where(x => x.Username == currentUsername)
                .FirstOrDefaultAsync(cancellationToken);
            if (person is null)
            {
                throw new RestException(
                    HttpStatusCode.NotFound,
                    new { User = Constants.NOT_FOUND }
                );
            }

            person.Username = message.User.Username ?? person.Username;
            person.Email = message.User.Email ?? person.Email;
            person.Bio = message.User.Bio ?? person.Bio;
            person.Image = message.User.Image ?? person.Image;

            if (!string.IsNullOrWhiteSpace(message.User.Password))
            {
                var salt = Guid.NewGuid().ToByteArray();
                person.Hash = await _passwordHasher.Hash(message.User.Password, salt);
                person.Salt = salt;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<Domain.Person, UserDto>(person);
        }
    }
}
