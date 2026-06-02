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


    [HttpPost]
    public async Task<ActionResult<Alarm>> Create([FromBody] Alarm request)
    {
        db.Alarms.Add(request);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Create), new { id = request.Id }, request);
    }

    [HttpPut]
    public async Task<Alarm> Update([FromBody] Alarm request)
    {
        var item = await db.Alarms.Where(x => x.Id == request.Id).SingleAsync();
        item.Title = request.Title;
        item.Time = request.Time;
        await db.SaveChangesAsync();
        return item;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await db.Alarms.Where(x => x.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }
}