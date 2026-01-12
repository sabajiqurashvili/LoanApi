using System.Security.Claims;
using BankApp_API.services;
using BankApp_API.services.Models;
using BankApp_Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp_API.Controllers;

[ApiController]
[Route("API/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
    {
        try
        {
            var validation = new Validations.RegisterDtoValidator();
            var validationResults = validation.Validate(registerDto);
            if (!validationResults.IsValid)
            {
                var errors = validationResults.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }

            var result = await _userService.RegisterUser(registerDto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var validataion = new Validations.LoginDtoValidator();
            var validationResult = validataion.Validate(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }

            var result = await _userService.LoginUserAsync(loginDto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new Dictionary<string,string>(){{"message" , e.Message }});
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result == null)
                return NotFound("User not found");

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = Roles.User)]
    [HttpPost("loans")]
    public async Task<IActionResult> RequestLoan([FromBody] LoanDto loanDto)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name).Value);
        var CurrentUser = await _userService.GetUserByIdAsync(currentUserId);
        if (CurrentUser.IsBlocked)
        {
            return Forbid("You are Blocked So you can't request loan");
        }

        var validation = new Validations.LoanDtoValidator();
        var validationResult = validation.Validate(loanDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => error.ErrorMessage);
            return BadRequest(errors);
        }

        var loan = await _userService.RequestLoan(currentUserId, loanDto);
        return Ok(loan);

    }

    [Authorize(Roles = Roles.User)]
    [HttpGet("loans/my")]
    public async Task<IActionResult> GetLoans()
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name).Value);
        var loans = await _userService.GetLoans(currentUserId);
        return Ok(loans);
    }

    [Authorize(Roles = Roles.User)]
    [HttpPut("loans/{loanId}")]
    public async Task<IActionResult> UpdateLoan(int loanId,[FromBody] LoanDto loanDto)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name).Value);
        var validation = new Validations.LoanDtoValidator();
        var validationResult = validation.Validate(loanDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => error.ErrorMessage);
            return BadRequest(errors);
        }

        try
        {
            var loan = await _userService.UpdateLoan(currentUserId, loanId, loanDto);
            return Ok(loan);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }

    [Authorize(Roles = Roles.User)]
    [HttpDelete("loans/{loanId}")]
    public async Task<IActionResult> DeleteLoan(int loanId)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name).Value);
        try
        {
            await _userService.DeleteLoan(currentUserId, loanId);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Loan has been deleted successfully");
    }
    

}
