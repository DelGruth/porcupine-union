using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Contract.Responses;

public class GroupListResponse
{
    public IEnumerable<GroupDto> Groups { get; set; } = new List<GroupDto>();
}
