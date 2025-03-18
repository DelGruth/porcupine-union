namespace UserAccessSystem.Domain;

public class Permission : BaseDomainObj
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}
