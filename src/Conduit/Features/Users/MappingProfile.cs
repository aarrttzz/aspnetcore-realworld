using AutoMapper;
using Conduit.DTO;

namespace Conduit.Features.Users;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Person, UserDto>(MemberList.None);
    }
}
