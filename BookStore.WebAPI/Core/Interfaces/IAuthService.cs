using BookStore.WebAPI.Core.DTOs;

namespace BookStore.WebAPI.Core.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResponseDTO> SeedRolesAsync();

    Task<AuthServiceResponseDTO> RegisterAsync(RegisterDTO registerDTO);
    Task<AuthServiceResponseDTO> LoginAsync(LoginDTO loginDTO);
    Task<AuthServiceResponseDTO> MakeAdminAsync(UpdatePermissionDTO updatePermissionDTO);
    Task<AuthServiceResponseDTO> MakeOwnerAsync(UpdatePermissionDTO updatePermissionDTO);

}
