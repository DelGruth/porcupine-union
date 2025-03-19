using UserAccessSystem.Domain.Common;

namespace UserAccessSystem.Domain.User;

public class UserGroupMembership : BaseDomainObj
{
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required Guid GroupId { get; set; }
    public Group.Group? Group { get; set; }
}
