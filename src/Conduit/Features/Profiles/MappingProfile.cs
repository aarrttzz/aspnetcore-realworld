using AutoMapper;
using Conduit.DTO;

namespace Conduit.Features.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Person, ProfileDto>(MemberList.None);
    }
}
