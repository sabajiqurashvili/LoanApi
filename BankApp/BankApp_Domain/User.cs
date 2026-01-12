namespace BankApp_Domain;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public decimal Salary { get; set; }
    public bool IsBlocked { get; set; }
    public string Role { get; set; }
    
    //1 to 1 [user to LoggedUserInfo]
    public LoggedUserInfo LoggedUserInfo { get; set; }
    
    //many to one [User have 1 accountant]
    public Accountant accountant { get; set; }
    public int? AccountantId { get; set; }
    
    //1 to many [User can have more than 1 Loan]
    public List<Loan> Loans { get; set; }
}

public static class Roles
{
    public const string User = "User";
    public const string Accountant = "Accountant";
    public const string Admin = "Admin";
}