using UserAccessSystem.Domain.Common;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.UserDetail;

namespace UserAccessSystem.Domain.User;

public class User : BaseDomainObj
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required LockStatus LockStatus { get; set; }
    public required string Password { get; set; }
    public IEnumerable<UserGroupMembership> Groups { get; set; } =
        Array.Empty<UserGroupMembership>();

    public IEnumerable<UserPermission> UserPermissions { get; set; } =
        ArraySegment<UserPermission>.Empty;
}
