using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Domain.Permissions;

public class GroupPermission : BaseDomainObj
{
    public required Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
    public required Guid GroupId { get; set; }
    public Group.Group Group { get; set; }
}
