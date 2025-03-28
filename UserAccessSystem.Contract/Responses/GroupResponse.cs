using UserAccessSystem.Contract.Dtos;

namespace UserAccessSystem.Contract.Responses;

public record GroupResponse
{
    public required GroupDto Group { get; init; }
}