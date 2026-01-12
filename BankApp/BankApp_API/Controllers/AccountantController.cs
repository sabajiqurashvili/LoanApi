using BankApp_API.services;
using BankApp_API.services.Models;
using BankApp_Data;
using BankApp_Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountantController : Controller
{
    private readonly IAccountantService _accountantService;

    public AccountantController(IAccountantService accountantService) => _accountantService = accountantService;
   
    
    [Authorize(Roles = Roles.Accountant)]
    [HttpGet("User/Loans/{userId}")]
    public async Task<IActionResult> SeeLoans(int userId)
    {
        if(userId < 0) return BadRequest("Invalid userId");
        try
        {
            var loans = await _accountantService.SeeLoans(userId);
            if(loans.Count == 0) return NotFound("No loans found");
            return Ok(loans);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = Roles.Accountant)]
    [HttpPut("User/Loans/{userId},{loanId}")]
    public async Task<IActionResult> UpdateLoan(int userId, int loanId, [FromBody] LoanDtoForAccountant loanDto)
    {
        if(userId < 0 || loanId<0) return BadRequest("userId and loanId cant be null");
        try
        {
            await _accountantService.UpdateLoan(userId, loanId, loanDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok($"loan status changed to {loanDto.Status}");
    }

    [Authorize(Roles = Roles.Accountant)]
    [HttpDelete("User/Loans/{userId},{loanId}")]
    public async Task<IActionResult> DeleteLoan(int userId, int loanId)
    {
        if(userId < 0 || loanId<0) return BadRequest("userId and loanId cant be less than 0");
        try
        {
            await _accountantService.DeleteLoan(userId, loanId);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Loan has been deleted successfully");
    }

    [Authorize(Roles = Roles.Accountant)]
    [HttpPut("Users/Block/{UserId}")]
    public async Task<IActionResult> BlockUser(int userId)
    {
        if(userId < 0) return BadRequest("Invalid userId");
        try
        {
            await _accountantService.BlockUser(userId);
        }
        catch (Exception e)
        {
           return BadRequest(e.Message);
        }
        return Ok($"UserID : {userId} has been blocked");
    }
}