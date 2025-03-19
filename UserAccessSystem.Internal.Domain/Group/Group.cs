using UserAccessSystem.Domain.Common;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Domain.Group;

public class Group : BaseDomainObj
{
    public required string Name { get; set; }
    public required string? Description { get; set; }

    public IEnumerable<UserGroupMembership> Users { get; set; } = new List<UserGroupMembership>();

    public IEnumerable<UserPermission> MemberPermissions { get; set; } = new List<UserPermission>();

    public IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        new List<GroupPermission>();
}
