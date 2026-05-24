using System.ComponentModel.DataAnnotations;

namespace TodoAppApi.Models;

public class Alarm
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public DateTime Time { get; set; } = DateTime.UtcNow;
}
