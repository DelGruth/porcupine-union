namespace UserAccessSystem.Contract.Requests;

public record AddUserPermissionRequest
{
    public required Guid UserId { get; init; }
    public required Guid PermissionId { get; init; }
    public required Guid GroupId { get; init; }
}