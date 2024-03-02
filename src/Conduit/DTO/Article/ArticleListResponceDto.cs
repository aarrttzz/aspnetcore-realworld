using System.Collections.Generic;
namespace Conduit.DTO;

public record ArticleListResponceDto
{
    public List<ArticleDto> Articles { get; set; } = new();
    public int ArticlesCount { get; set; }
}
