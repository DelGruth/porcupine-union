namespace UserAccessSystem.Contract.Requests;

public record AddPermissionToGroupRequest
{
    public required Guid PermissionId { get; set; }
    public required Guid GroupId { get; set; }
}