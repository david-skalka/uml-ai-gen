namespace TodoAppApi.Models;

public record GroupByNameOutput
{
    public required string Name { get; init; }

    public int Count { get; init; }
}