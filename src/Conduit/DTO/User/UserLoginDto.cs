namespace Conduit.DTO;

public record UserLoginDto
{
    public string? Email { get; set; }

    public string? Password { get; set; }
}
