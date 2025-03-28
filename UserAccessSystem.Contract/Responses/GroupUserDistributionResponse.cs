namespace UserAccessSystem.Contract.Responses;

public record GroupUserDistributionResponse
{
    public required IDictionary<string, int> Distribution { get; init; }
}