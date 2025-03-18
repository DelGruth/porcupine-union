namespace UserAccessSystem.Domain;

public class UserGroup : BaseDomainObj
{
    public required Guid UserId { get; set; }
    public required Guid GroupId { get; set; }

    public required User User { get; set; }
    public required Group Group { get; set; }
}
