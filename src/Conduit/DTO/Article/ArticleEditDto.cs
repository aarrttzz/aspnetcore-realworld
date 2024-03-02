namespace Conduit.DTO;

public record ArticleEditDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Body { get; set; }

    public string[]? TagList { get; set; }
}
