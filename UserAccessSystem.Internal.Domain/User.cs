namespace UserAccessSystem.Domain;

public class User : BaseDomainObj
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required LockStatus LockStatus { get; set; }
    public IEnumerable<UserGroupMembership> Groups { get; set; } =
        Array.Empty<UserGroupMembership>();

    public IEnumerable<UserPermission> UserPermissions { get; set; } =
        ArraySegment<UserPermission>.Empty;
}
