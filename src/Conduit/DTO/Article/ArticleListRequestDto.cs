using MediatR;
namespace Conduit.DTO;

public record ArticleListRequestDto(
    string Tag,
    string Author,
    string FavoritedUsername,
    int? Limit,
    int? Offset,
    bool IsFeed = false
);
