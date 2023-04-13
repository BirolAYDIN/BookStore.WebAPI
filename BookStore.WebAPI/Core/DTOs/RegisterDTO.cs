using System.ComponentModel.DataAnnotations;

namespace BookStore.WebAPI.Core.DTOs;

public class RegisterDTO
{

    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage ="User name is required.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "E-mail is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;
}
