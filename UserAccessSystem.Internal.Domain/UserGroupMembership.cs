namespace UserAccessSystem.Domain;

public class UserGroupMembership : BaseDomainObj
{
    public required Guid UserId { get; set; }
    public required User User { get; set; }
    public required Guid GroupId { get; set; }
    public required Group Group { get; set; }
}
