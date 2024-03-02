using System;

namespace Conduit.DTO;

public record ArticleDto
{
    public string? Slug { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Body { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool Favorited { get; set; } = false;

    public int FavoritesCount { get; set; } = 0;

    public string[] TagList { get; set; } =  Array.Empty<string>();
}
