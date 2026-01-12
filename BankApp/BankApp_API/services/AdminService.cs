using BankApp_Data;
using BankApp_Domain;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;

namespace BankApp_API.services;

public class AdminService
{
    private readonly BankAppContext _context;

    public AdminService(BankAppContext context) => _context = context;

    public async Task<string> PromoteToAccountant(int userId)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null)
            throw new Exception("User not found to promote");
        
        if (user.Role == Roles.Accountant)
            return $"User with ID {userId} is already an accountant";
        
        user.Role = Roles.Accountant.ToString();
        _context.Users.Update(user);
        
        var accountant = new Accountant()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
        _context.Accountants.Add(accountant);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException dbEx)
        {
            throw new Exception($"EF Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"General Error: {ex.Message}");
        }

        return $"User with ID {userId} promoted to accountant";
    }

   
}