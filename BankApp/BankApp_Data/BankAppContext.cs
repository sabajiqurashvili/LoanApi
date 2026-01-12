using BankApp_Domain;
using Microsoft.EntityFrameworkCore;

namespace BankApp_Data;

public class BankAppContext : DbContext
{
    public BankAppContext(DbContextOptions<BankAppContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<LoggedUserInfo> LoggedUserInfos { get; set; }
    public DbSet<Accountant> Accountants { get; set; }
    public DbSet<Loan> Loans { get; set; }
}