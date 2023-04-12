using System.ComponentModel.DataAnnotations;

namespace BookStore.WebAPI.Core.DTOs
{
    public class UpdatePermissionDTO
    {
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; } = null!;
    }
}
