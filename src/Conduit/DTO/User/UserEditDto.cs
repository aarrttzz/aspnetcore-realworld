namespace Conduit.DTO;

public record UserEditDto
{
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Bio { get; set; }

    public string? Image { get; set; }
}
