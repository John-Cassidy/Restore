namespace Restore.API.DTOs;

public record UserDto(string Email, string Token, BasketDto Basket);
