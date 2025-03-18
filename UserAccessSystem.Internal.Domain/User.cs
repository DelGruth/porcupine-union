namespace UserAccessSystem.Domain;

public class User : BaseDomainObj
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required LockStatus LockStatus { get; set; }
}
