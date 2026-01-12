namespace BankApp_Domain;

public class Loan
{
    public int Id { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int LoanPeriodInMonths { get; set; }
    public Status Status { get; set; }
    
    
    // many to one [loan has one owner(User)]
    public User User { get; set; }
    public int UserId { get; set; }
    
    
}

public enum LoanType
{
    QuickLoan ,
    AutoLoan ,
    Installment
}

public enum Currency
{
   GEL ,
   USD ,
   EUR 
}

public enum Status
{
    Proccesing ,
    Approved ,
    Rejected 
}