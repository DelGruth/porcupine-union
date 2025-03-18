namespace UserAccessSystem.Contract.Responses;

public readonly struct GetAllGroupsResponse
{
    public string[] GroupNames { get; init; }
}
