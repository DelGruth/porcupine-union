using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Domain.Permissions;

public class Permission : BaseDomainObj
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool ReadOnly { get; set; }
    public required bool WriteOnly { get; set; }
    public IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        ArraySegment<GroupPermission>.Empty;

    public IEnumerable<UserPermission> UserPermissions { get; set; } =
        ArraySegment<UserPermission>.Empty;
}
