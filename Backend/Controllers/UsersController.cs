namespace WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Claims;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenValidator _jwtTokenValidator;

    public UsersController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _jwtTokenValidator = new JwtTokenValidator(configuration);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        try
        {
            var newUser = await _userService.RegisterUserAsync(request);
            return Ok(newUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Authenticate")]
    public async Task<IActionResult> AuthenticateUser([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.AuthenticateUserAsync(request);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferBalance([FromBody] TransferModel transferData)
    {
        try
        {

            await _userService.TransferBalance(transferData.SenderMobile, transferData.RecipientMobile, transferData.Amount);
            return Ok($"Amount {transferData.Amount} transfered successfully to Phone Number {transferData.RecipientMobile}.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
