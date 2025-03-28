using UserAccessSystem.Domain.Group;

namespace UserAccessSystem.Contract.Dtos;

public class GroupDto
{
    public Group Group { get; set; }

    public GroupDto() { }

    public GroupDto(Group group)
    {
        this.Group = group;
    }
}
