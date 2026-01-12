using BankApp_API.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : Controller
{
    private readonly AdminService _service;

    public AdminController(AdminService service) => _service = service;
   
    
    
    
    [Authorize(Roles = "Admin")]
    [HttpPut("AccountantPromotion/{userId}")]
    public async Task<IActionResult> PromoteToAccountant(int userId)
    {
        try
        {
            var result = await _service.PromoteToAccountant(userId);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok("Promoted to accountant");
    }
}