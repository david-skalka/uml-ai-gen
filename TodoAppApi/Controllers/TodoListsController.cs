using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAppApi.Models;

namespace TodoAppApi.Controllers;

[ApiController]
[Route("api/todo-lists")]
public class TodoListsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoList>>> GetAll()
    {
        return await db.TodoLists
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    [HttpPost]
    public async Task<ActionResult<TodoList>> Create([FromBody] TodoList request)
    {
        db.TodoLists.Add(request);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Create), new { id = request.Id }, request);
    }

    [HttpPut]
    public async Task<TodoList> Update([FromBody] TodoList request)
    {
        var item = await db.TodoLists.Where(x => x.Id == request.Id).SingleAsync();
        item.Name = request.Name;
        item.Description = request.Description;
        await db.SaveChangesAsync();
        return item;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await db.TodoLists.Where(x => x.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }

    [HttpPost("group-by-name")]
    public async Task<ActionResult<IEnumerable<GroupByNameOutput>>> GroupByName([FromBody] GroupByNameInput input)
    {
        var query = db.TodoLists.AsNoTracking().AsQueryable();
        if (!input.IncludeArchived)
            query = query.Where(x => !x.IsArchived);

        var results = await query
            .GroupBy(x => x.Name)
            .Select(g => new GroupByNameOutput
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Name)
            .ToListAsync();
        _ = results.Select(x => x.Count).ToList();

        return results;
    }
}