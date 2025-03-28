namespace UserAccessSystem.Contract.Requests;

public record DeleteRequest
{
    public required Guid Id { get; init; }
}