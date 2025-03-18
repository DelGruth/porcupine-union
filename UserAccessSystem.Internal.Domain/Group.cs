namespace UserAccessSystem.Domain;

public class Group : BaseDomainObj
{
    public required string Name { get; set; }
    public required string? Description { get; set; }
}
