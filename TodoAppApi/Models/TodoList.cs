using System.ComponentModel.DataAnnotations;

namespace TodoAppApi.Models;

public class TodoList
{
    [Required] public int Id { get; init; }

    [Required] [MaxLength(255)] public string Name { get; set; } = string.Empty;

    [MaxLength(2000)] public string Description { get; set; } = string.Empty;

    public bool IsArchived { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}