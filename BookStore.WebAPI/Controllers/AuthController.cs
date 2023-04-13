﻿using BookStore.WebAPI.Core.DTOs;
using BookStore.WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    // Route for seeding my roles to db
    [HttpPost("seed-roles")]
    public async Task<IActionResult> SeedRoles()
    {
        bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);

        if (isUserRoleExists && isAdminRoleExists && isOwnerRoleExists)
            return Ok("Roles seeding is already done.");

        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

        return Ok("Role seeding done successfully.");
    }

    // Route -> Register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var isExistsUser = await _userManager.FindByNameAsync(registerDTO.UserName);

        if (isExistsUser is not null)
            return BadRequest("User name already exists.");

        IdentityUser newUser = new IdentityUser()
        {
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
            return BadRequest(errorString);
        }

        await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

        return Ok("User Created Successfully");
    }

    // Route -> Login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByNameAsync(loginDTO.UserName);

        if(user is null)
            return Unauthorized("Invalid Credentials");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!isPasswordCorrect)
            return Unauthorized("Invalid Creadentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("JWTID", Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateNewJsonWebToken(authClaims);

        return Ok(token);

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

    // Route => User -> Admin
    [HttpPost("make-admin")]
    public async Task<IActionResult> MakeAdmin(UpdatePermissionDTO updatePermissionDTO)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDTO.UserName);

        if (user is null)
            return BadRequest("Invalid User name.");

        await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

        return Ok("User is now an ADMIN.");
    }


    // Route => User -> Owner
    [HttpPost("make-owner")]
    public async Task<IActionResult> MakeOwner(UpdatePermissionDTO updatePermissionDTO)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDTO.UserName);

        if (user is null)
            return BadRequest("Invalid User name.");

        await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

        return Ok("User is now an OWNER.");
    }
}
