using BankApp_Domain;

namespace BankApp_API.services.Models;

public class LoanDto
{
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int LoanPeriodInMonths { get; set; }


}