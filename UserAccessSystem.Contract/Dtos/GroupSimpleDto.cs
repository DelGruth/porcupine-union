using UserAccessSystem.Domain.Group;

namespace UserAccessSystem.Contract.Dtos;

public class GroupSimpleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public GroupSimpleDto() { }

    public GroupSimpleDto(Group group)
    {
        Id = group.Id;
        Name = group.Name;
        Description = group.Description;
    }
}