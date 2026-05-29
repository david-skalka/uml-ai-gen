using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAppApi.Models;

namespace TodoAppApi.Controllers;

[ApiController]
[Route("api/alarms")]
public class AlarmsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alarm>>> GetAll()
    {
        return await db.Alarms
            .AsNoTracking()
            .OrderBy(x => x.Time)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Alarm>> GetById(int id)
    {
        var item = await db.Alarms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<Alarm>> Create([FromBody] CreateAlarmRequest request)
    {
        var item = new Alarm
        {
            Title = request.Title,
            Time = request.Time
        };

        db.Alarms.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Alarm>> Update(int id, [FromBody] UpdateAlarmRequest request)
    {
        var item = await db.Alarms.SingleOrDefaultAsync(x => x.Id == id);
        if (item is null)
            return NotFound();

        item.Title = request.Title;
        item.Time = request.Time;
        await db.SaveChangesAsync();
        return item;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await db.Alarms.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }
}

public record CreateAlarmRequest
{
    [Required]
    public string Title { get; init; } = string.Empty;

    public DateTime Time { get; init; }
}

public record UpdateAlarmRequest
{
    [Required]
    public string Title { get; init; } = string.Empty;

    public DateTime Time { get; init; }
}
