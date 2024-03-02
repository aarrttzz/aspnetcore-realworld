using System.Linq;
using AutoMapper;
using Conduit.DTO;

namespace Conduit.Features.Articles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Article, ArticleDto>(MemberList.None)
            .ForMember(
                dest => dest.TagList,
                opt => opt.MapFrom(src => src.ArticleTags.Where(x => x.TagId != null).Select(x => x.TagId!).ToList())
            )
            .ForMember(
                dest => dest.Favorited,
                opt => opt.MapFrom(src => src.ArticleFavorites.Any())
            )
            .ForMember(
                dest => dest.FavoritesCount,
                opt => opt.MapFrom(src => src.ArticleFavorites.Count())
            );
    }
}
