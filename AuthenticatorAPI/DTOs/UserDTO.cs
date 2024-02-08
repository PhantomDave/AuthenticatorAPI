namespace AuthenticatorAPI.DTOs;

public record UserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}