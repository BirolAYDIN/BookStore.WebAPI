using System.ComponentModel.DataAnnotations;

namespace BookStore.WebAPI.Core.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage ="User name is required.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "E-mail is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;
}
