using System.ComponentModel.DataAnnotations;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoList>> GetById(int id)
    {
        var item = await db.TodoLists.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<TodoList>> Create([FromBody] CreateTodoListRequest request)
    {
        var item = new TodoList
        {
            Name = request.Name,
            Description = request.Description
        };

        db.TodoLists.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoList>> Update(int id, [FromBody] UpdateTodoListRequest request)
    {
        var item = await db.TodoLists.SingleOrDefaultAsync(x => x.Id == id);
        if (item is null)
            return NotFound();

        item.Name = request.Name;
        item.Description = request.Description;
        await db.SaveChangesAsync();
        return item;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await db.TodoLists.Where(x => x.Id == id).ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
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

        return results;
    }
}

public record CreateTodoListRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}

public record UpdateTodoListRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
