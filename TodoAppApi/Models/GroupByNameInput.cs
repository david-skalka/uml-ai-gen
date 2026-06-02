namespace TodoAppApi.Models;

public record GroupByNameInput
{
    public bool IncludeArchived { get; init; }
}