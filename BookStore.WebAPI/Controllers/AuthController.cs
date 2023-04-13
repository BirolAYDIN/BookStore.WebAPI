using BookStore.WebAPI.Core.DTOs;
using BookStore.WebAPI.Core.Entities;
using BookStore.WebAPI.Core.Interfaces;
using BookStore.WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Route for seeding my roles to db
    [HttpPost("seed-roles")]
    public async Task<IActionResult> SeedRoles()
    {
        var seedRoles = await _authService.SeedRolesAsync();
        return Ok(seedRoles);
    }

    // Route -> Register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var registerResult = await _authService.RegisterAsync(registerDTO);

        if (registerResult.IsSuccess) 
            return Ok(registerResult);

        return BadRequest(registerResult);
    }

    // Route -> Login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO)
    { 
        var loginResult = await _authService.LoginAsync(loginDTO);

        if (loginResult.IsSuccess)
            return Ok(loginResult);

        return Unauthorized(loginResult);
    }

    

    // Route => User -> Admin
    [HttpPost("make-admin")]
    public async Task<IActionResult> MakeAdmin(UpdatePermissionDTO updatePermissionDTO)
    {
       var operationResult = await _authService.MakeAdminAsync(updatePermissionDTO);

        if (operationResult.IsSuccess)
            return Ok(operationResult);

        return BadRequest(operationResult);
    }


    // Route => User -> Owner
    [HttpPost("make-owner")]
    public async Task<IActionResult> MakeOwner(UpdatePermissionDTO updatePermissionDTO)
    {
        var operationResult = await _authService.MakeOwnerAsync(updatePermissionDTO);

        if (operationResult.IsSuccess)
            return Ok(operationResult);

        return BadRequest(operationResult);
    }
}
