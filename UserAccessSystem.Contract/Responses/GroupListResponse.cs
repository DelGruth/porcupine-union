using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Contract.Responses;

public record GroupListResponse
{
    public required IEnumerable<GroupDto> Groups { get; init; }
}