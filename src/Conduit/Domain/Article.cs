using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Conduit.Domain;

public class Article
{
    public int ArticleId { get; set; }

    public string? Slug { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Body { get; set; }

    public Person? Author { get; set; }

    public List<Comment> Comments { get; set; } = new();

    public List<ArticleTag> ArticleTags { get; set; } = new();

    public List<ArticleFavorite> ArticleFavorites { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
