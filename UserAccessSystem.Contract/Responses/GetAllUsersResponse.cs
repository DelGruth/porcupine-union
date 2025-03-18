namespace UserAccessSystem.Contract.Responses;

public readonly struct GetAllUsersResponse
{
    public string[] Names { get; init; }
}
