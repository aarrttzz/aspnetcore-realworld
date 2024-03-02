using System;
namespace Conduit.DTO;

public class CommentDto
{
    public int CommentId { get; set; }

    public string? Body { get; set; }

    // public Person? Author { get; set; }

    // public int AuthorId { get; set; }

    // public Article? Article { get; set; }

    //[JsonIgnore]
    //public int ArticleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

