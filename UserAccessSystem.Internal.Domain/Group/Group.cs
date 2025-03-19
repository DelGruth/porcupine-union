using UserAccessSystem.Domain.Common;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Domain.Group;

public class Group : BaseDomainObj
{
    public required string Name { get; set; }
    public required string? Description { get; set; }

    public IEnumerable<UserGroupMembership> Users { get; set; } =
        ArraySegment<UserGroupMembership>.Empty;

    public IEnumerable<UserPermission> MemberPermissions { get; set; } =
        ArraySegment<UserPermission>.Empty;

    public IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        ArraySegment<GroupPermission>.Empty;
}
