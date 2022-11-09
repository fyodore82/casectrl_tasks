using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Tasks.Domain;
using Tasks.Models;

namespace Tasks.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ToDoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetToDoItems")]
        public IList<ToDoItemDto> Get()
        {
            return _context.ToDoItems.Select(i => new ToDoItemDto(i)).ToList();
        }

        [HttpPost(Name = "CreateToDoItem")]
        public IActionResult CreateToDoItem([FromBody] CreateToDoItemDto createToDoItemDto)
        {
            var AccountId = User.Claims.FirstOrDefault(Claim => Regex.Match(Claim.Type, "sid").Success);
            if (String.IsNullOrEmpty(createToDoItemDto.Title) || String.IsNullOrEmpty(User.Identity?.Name) || String.IsNullOrEmpty(AccountId?.Value)) {
                return BadRequest(new { message = "Title, user name or accountId is empty" });
            }
            var toDoItem = new ToDoItem()
            {
                Id = 0,
                Title = createToDoItemDto.Title,
                Description = createToDoItemDto.Description,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = User.Identity.Name,
                ModifedOn = null,
                ModifiedBy = null,
                AccountId = AccountId?.Value,
            };
            _context.ToDoItems.Add(toDoItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(CreateToDoItem), toDoItem);
        }

        [HttpPut(Name = "UpdateToDoItem")]
        [Route("{id}")]
        public IActionResult UpdateToDoItem(int id, [FromBody] CreateToDoItemDto createToDoItemDto)
        {
            var AccountId = User.Claims.FirstOrDefault(Claim => Regex.Match(Claim.Type, "sid").Success);
            if (String.IsNullOrEmpty(createToDoItemDto.Title) || String.IsNullOrEmpty(User.Identity?.Name) || String.IsNullOrEmpty(AccountId?.Value))
            {
                return BadRequest(new { message = "Title, user name or accountId is empty" });
            }
            var toDoItem = _context.ToDoItems.SingleOrDefault(item => item.Id == id);
            if (toDoItem == null) return NotFound();
            
            toDoItem.Title = createToDoItemDto.Title;
            toDoItem.Description = createToDoItemDto.Description;
            toDoItem.ModifedOn = DateTime.Now.ToUniversalTime();
            toDoItem.ModifiedBy = User.Identity.Name;
            _context.SaveChanges();
            return Ok(new ToDoItemDto(toDoItem));
        }

        [HttpDelete(Name = "DeleteToDoItem")]
        [Route("{id}")]
        public IActionResult DeleteToDoItem(int id)
        {
            var toDoItem = _context.ToDoItems.SingleOrDefault(item => item.Id == id);
            if (toDoItem == null) return NotFound();

            _context.Remove(toDoItem);
            return Ok();
        }

    }
}
