namespace UserAccessSystem.Contract.Requests;

public record UpdateUserRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
}
