namespace BankApp_Domain;

public class Accountant
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    
    // 1 to many[1 accountant can handle many users]
    public List<User> Users { get; set; }
}