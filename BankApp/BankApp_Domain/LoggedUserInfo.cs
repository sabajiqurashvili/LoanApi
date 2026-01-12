namespace BankApp_Domain;

public class LoggedUserInfo
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    
    //1 to 1
    public int UserId { get; set; }
    public User User { get; set; }
    
    
}