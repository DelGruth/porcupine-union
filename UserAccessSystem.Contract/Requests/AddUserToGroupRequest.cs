namespace UserAccessSystem.Contract.Requests;

public record AddUserToGroupRequest
{
    public required Guid UserId { get; set; }
    public required Guid GroupId { get; set; }
}