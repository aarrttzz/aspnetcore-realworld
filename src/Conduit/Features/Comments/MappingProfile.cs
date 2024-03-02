using AutoMapper;
using Conduit.DTO;

namespace Conduit.Features.Comments;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Comment, CommentDto>(MemberList.None);
    }
}
