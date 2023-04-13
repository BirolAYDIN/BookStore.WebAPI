namespace BookStore.WebAPI.Core.DTOs;

public class AuthServiceResponseDTO
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;
}
