using BankApp_API.services.Models;
using BankApp_Domain;

namespace BankApp_API.services;

public interface IUserService
{
    public Task<User> RegisterUser(RegisterDto registerDto);
    public Task<LoginResponse> LoginUserAsync(LoginDto loginDto);
    public Task<User> GetUserByIdAsync(int id);
    public Task<Loan> RequestLoan(int userId,LoanDto loanDto);
    public Task<List<Loan>> GetLoans(int userId);
    public Task<Loan> UpdateLoan(int userId,int loanId, LoanDto loanDto);
    public Task DeleteLoan(int userId,int loanId);

}