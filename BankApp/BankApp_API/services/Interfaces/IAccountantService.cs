using BankApp_API.services.Models;
using BankApp_Domain;

namespace BankApp_API.services;

public interface IAccountantService
{
    public Task<List<Loan>> SeeLoans(int userId);
    public Task<Loan> UpdateLoan(int userId,int loanId,LoanDtoForAccountant loanDto);
    public Task DeleteLoan(int userId,int loanId);
    public Task BlockUser(int userId);
}