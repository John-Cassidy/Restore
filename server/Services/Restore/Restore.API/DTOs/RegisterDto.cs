namespace Restore.API.DTOs;

public record RegisterDto(string Username, string Password, string Email) : LoginDto(Username, Password);
