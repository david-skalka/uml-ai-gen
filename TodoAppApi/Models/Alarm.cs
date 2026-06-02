using System.ComponentModel.DataAnnotations;

namespace TodoAppApi.Models;

public class Alarm
{
    [Required] public int Id { get; init; }

    [Required] [MaxLength(255)] public string Title { get; set; } = string.Empty;

    public DateTime Time { get; set; } = DateTime.UtcNow;
}