namespace Conduit.DTO;

public record UserCreateDto
{
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}
