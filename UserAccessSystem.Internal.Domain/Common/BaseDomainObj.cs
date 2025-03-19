namespace UserAccessSystem.Domain.Common;

public class BaseDomainObj
{
    public Guid Id { get; set; }
    public DateTime CreatedAtDateTime { get; set; }
    public DateTime EditedDateTime { get; set; }
    public Guid EditedById { get; set; }
    public long Version { get; set; }
    public bool IsDeleted { get; set; }
}
