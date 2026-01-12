namespace BankApp_API.services.Models;

public class RegisterDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public decimal Salary { get; set; }
    public int? AccountantId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
}