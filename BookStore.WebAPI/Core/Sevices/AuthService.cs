using BookStore.WebAPI.Core.DTOs;
using BookStore.WebAPI.Core.Entities;
using BookStore.WebAPI.Core.Interfaces;
using BookStore.WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.WebAPI.Core.Sevices;

public class AuthService : IAuthService
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthServiceResponseDTO> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByNameAsync(loginDTO.UserName);

        if (user is null)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "Invalid Credentials"
            };

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!isPasswordCorrect)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "Invalid Creadentials"
            };

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("JWTID", Guid.NewGuid().ToString()),
            new Claim("First Name", user.FirstName),
            new Claim("Last Name", user.LastName),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateNewJsonWebToken(authClaims);

        return new AuthServiceResponseDTO()
        {
            IsSuccess = true,
            Message = token
        };
    }

    public async Task<AuthServiceResponseDTO> MakeAdminAsync(UpdatePermissionDTO updatePermissionDTO)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDTO.UserName);

        if (user is null)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "Invalid User name."
            };
        
        await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

        return new AuthServiceResponseDTO()
        {
            IsSuccess = true,
            Message = "User is now an ADMIN."
        }; 
    }

    public async Task<AuthServiceResponseDTO> MakeOwnerAsync(UpdatePermissionDTO updatePermissionDTO)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDTO.UserName);

        if (user is null)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "Invalid User name."
            };
        

        await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

        return new AuthServiceResponseDTO()
        {
            IsSuccess = true,
            Message = "User is now an OWNER."
        };
    }

    public async Task<AuthServiceResponseDTO> RegisterAsync(RegisterDTO registerDTO)
    {
        bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);

        if (!isUserRoleExists && !isAdminRoleExists && !isOwnerRoleExists)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "Roles are not seeded. Please do role seeding first."
            };

        // ---------------------------------------------------------
        var isExistsUser = await _userManager.FindByNameAsync(registerDTO.UserName);

        if (isExistsUser is not null)
            return new AuthServiceResponseDTO()
            {
                IsSuccess = false,
                Message = "User name already exists."
            };

        ApplicationUser newUser = new ApplicationUser()
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Email = registerDTO.Email,
            UserName = registerDTO.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerDTO.Password);

        if (!createUserResult.Succeeded)
        {
            var errorString = "User creation failed beacause: ";
            foreach (var error in createUserResult.Errors)
            {
                errorString += "# " + error.Description;
            }
            return new AuthServiceResponseDTO()
            {
                IsSuccess = true,
                Message = errorString
            };
            
        }

        await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

        return new AuthServiceResponseDTO()
        {
            IsSuccess = true,
            Message = "User Created Successfully"
        };
       
    }

    public async Task<AuthServiceResponseDTO> SeedRolesAsync()
    {
        bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);

        if (isUserRoleExists && isAdminRoleExists && isOwnerRoleExists)
            
            return new AuthServiceResponseDTO()
            {
                IsSuccess = true,
                Message = "Roles seeding is done."
            };

        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

        return new AuthServiceResponseDTO()
        {
            IsSuccess = true,
            Message = "Role seeding done successfully."
        };
    }

    private string GenerateNewJsonWebToken(List<Claim> claims)
    {
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
        return token;
    }
}
