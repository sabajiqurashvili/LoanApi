using BankApp_Data;
using BankApp_Domain;

namespace BankApp_API.services.Models;

public class AdminSeeder
{
    private readonly BankAppContext _context;

    public AdminSeeder(BankAppContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Users.Any(u => u.UserName == "admin"))
        {
            var user = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Age = 21,
                UserName = "admin",
                Role = Roles.Admin
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var login = new LoggedUserInfo
            {
                UserId = user.Id,
                Username = "admin",
                HashedPassword = HashPassword("Admin123")
            };

            _context.LoggedUserInfos.Add(login);
            _context.SaveChanges();
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
