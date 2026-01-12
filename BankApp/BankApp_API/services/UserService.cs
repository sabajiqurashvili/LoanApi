using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankApp_API.services.Models;
using BankApp_Data;
using BankApp_Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankApp_API.services;

public class UserService : IUserService
{
    private readonly BankAppContext _context;
    private readonly AppSettings _settings;
    private readonly ILogger<UserService> _logger;

    public UserService(BankAppContext context, IOptions<AppSettings> options, ILogger<UserService> logger)
    {
        _context = context;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<User> RegisterUser(RegisterDto registerDto)
    {
        if (await _context.LoggedUserInfos.AnyAsync(u => u.Username == registerDto.UserName))
        {
            _logger.LogError("Username {Username} already exists", registerDto.UserName);
            throw new Exception($"Username {registerDto.UserName} is already taken");
        }

        Accountant accountant = null;

        if (registerDto.AccountantId.HasValue)
        {
            accountant = await _context.Accountants
                .SingleOrDefaultAsync(a => a.Id == registerDto.AccountantId);

            if (accountant == null)
            {
                _logger.LogError("Accountant {AccountantId} not found", registerDto.AccountantId);
                throw new Exception($"Accountant {registerDto.AccountantId} not found");
            }

            _context.Entry(accountant).State = EntityState.Unchanged;
        }

        var user = new User()
        {
            UserName = registerDto.UserName,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Age = registerDto.Age,
            Salary = registerDto.Salary,
            accountant = accountant,
            Role = Roles.User
        };

        var loggedUser = new LoggedUserInfo()
        {
            Username = registerDto.UserName,
            HashedPassword = HashPassword(registerDto.Password),
            User = user,
        };

        _logger.LogInformation("Registering user {Username}", registerDto.UserName);

        _context.Users.Add(user);
        _context.LoggedUserInfos.Add(loggedUser);

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

        _logger.LogInformation("Registered user {Username}", registerDto.UserName);
        return user;
    }

    public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
    {
        if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            return null;

        var user = await _context.Users
            .Include(u => u.LoggedUserInfo)
            .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

        if (user == null)
        {
            _logger.LogInformation("Username {Username} not found", loginDto.Username);
            throw new Exception($"User {loginDto.Username} not found");
        }

        bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.LoggedUserInfo.HashedPassword);
        if (!passwordValid)
        {
            _logger.LogInformation("Password {Password} not valid", loginDto.Password);
            throw new Exception($"Password {loginDto.Password} not valid");
        }

        var token = GenerateToken(user);

        _logger.LogInformation("User {Username} logged in successfully", loginDto.Username);

        return new LoginResponse()
        {
            Token = token,
            Role = user.Role
        };
    }

    public async  Task<Loan> RequestLoan(int userId,LoanDto loanDto)
    {
        var loan = new Loan()
        {
            Amount = loanDto.Amount,
            Currency = loanDto.Currency,
            LoanPeriodInMonths = loanDto.LoanPeriodInMonths,
            LoanType = loanDto.LoanType,
            UserId = userId,
            Status = Status.Proccesing
        };
        _context.Loans.Add(loan);
        _logger.LogInformation("Added Loan from User : {UserId}", userId);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<List<Loan>> GetLoans(int userId)
    {
       var loans = await _context.Loans.Where(l => l.UserId == userId).ToListAsync();
       return loans;
    }

    public async Task<Loan> UpdateLoan(int userId, int loanId,LoanDto loanDto)
    {
        if (!await _context.Loans.AnyAsync(l => l.UserId == userId && l.Id == loanId))
        {
            _logger.LogError("Loan {LoanId} not found", loanId);
            throw new Exception($"Loan {loanId} not found");
        }
        
       var loan = await _context.Loans.FirstOrDefaultAsync(l => l.UserId == userId && l.Id == loanId);
       if (loan.Status != Status.Proccesing)
       {
           _logger.LogError("Loan {LoanId} status must be proccesing", loanId);
           throw new Exception("Loan status must be proccesing");
       }
       
       loan.LoanType = loanDto.LoanType;
       loan.Amount = loanDto.Amount;
       loan.Currency = loanDto.Currency;
       loan.LoanPeriodInMonths = loanDto.LoanPeriodInMonths;
       _context.Loans.Update(loan);
       _logger.LogInformation("Updated Loan from User : {UserId}", userId);
       await _context.SaveChangesAsync();
       return loan;
    }

    public async Task DeleteLoan(int userId, int loanId)
    {
        if (!await _context.Loans.AnyAsync(l => l.UserId == userId && l.Id == loanId))
        {
            _logger.LogError("Loan {LoanId} not found", loanId);
            throw new Exception($"Loan {loanId} not found");
        }
        
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.UserId == userId && l.Id == loanId);
        if (loan.Status != Status.Proccesing)
        {
            throw new Exception("Loan status must be proccesing");
        }
        _context.Loans.Remove(loan);
        _logger.LogInformation("Deleted Loan from User : {UserId}", userId);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private string GenerateToken(User user)
    {
        var tokenhandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Secret);

        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenhandler.CreateToken(TokenDescriptor);
        return tokenhandler.WriteToken(token);
    }
}