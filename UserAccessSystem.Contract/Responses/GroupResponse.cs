using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Contract.Responses;

public class GroupResponse
{
    public GroupDto Group { get; set; } = new();
}
