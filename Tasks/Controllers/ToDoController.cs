using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Tasks.Domain;
using Tasks.Models;
using Tasks.Services;

namespace Tasks.Controllers
{
    [Authorize]
    [Route("api/todo")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IRabbitMqClient _rabbitMqClient;
        private readonly IToDoRepository _toDoRepository;
        public ToDoController(IRabbitMqClient rabbitMqClient,
            IToDoRepository toDoRepository)
        {
            _rabbitMqClient = rabbitMqClient;
            _toDoRepository = toDoRepository;
        }

        [HttpGet]
        public IList<ToDoItemDto> Get()
        {
            return _toDoRepository.GetToDoItems().Select(i => new ToDoItemDto(i)).ToList();
        }

        [HttpPost]
        public IActionResult CreateToDoItem([FromBody] CreateToDoItemDto createToDoItemDto)
        {
            var AccountId = User.Claims.FirstOrDefault(Claim => Regex.Match(Claim.Type, "sid").Success);
            if (String.IsNullOrEmpty(createToDoItemDto.Title) || String.IsNullOrEmpty(User.Identity?.Name) || String.IsNullOrEmpty(AccountId?.Value)) {
                return BadRequest(new { message = "Title, user name or accountId is empty" });
            }
            var toDoItem = _toDoRepository.CreateToDoItem(createToDoItemDto, User.Identity.Name, AccountId.Value);
            _rabbitMqClient.SendMessage("ToDoItem", new ToDoRabbitMessage()
            {
                Action = ToDoRabbitActions.Create,
                OldItem = null,
                NewItem = toDoItem
            });
            return CreatedAtAction(nameof(CreateToDoItem), new ToDoItemDto(toDoItem));
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateToDoItem(int id, [FromBody] CreateToDoItemDto createToDoItemDto)
        {
            try
            {
                var AccountId = User.Claims.FirstOrDefault(Claim => Regex.Match(Claim.Type, "sid").Success);
                if (String.IsNullOrEmpty(createToDoItemDto.Title) || String.IsNullOrEmpty(User.Identity?.Name) || String.IsNullOrEmpty(AccountId?.Value))
                {
                    return BadRequest(new { message = "Title, user name or accountId is empty" });
                }
                ToDoItem originalToDoItem;
                var toDoItem = _toDoRepository.UpdateToDoItem(createToDoItemDto, User.Identity.Name, id, out originalToDoItem);
                _rabbitMqClient.SendMessage("ToDoItem", new ToDoRabbitMessage()
                {
                    Action = ToDoRabbitActions.Update,
                    OldItem = originalToDoItem,
                    NewItem = toDoItem
                });
                return Ok(new ToDoItemDto(toDoItem));
            }
            catch (ToDoItemNotFountException)
            {
                return NotFound(new {message = $"ToDo item #{id} not found."});
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteToDoItem(int id)
        {
            try
            {
                ToDoItem originalToDoItem;
                _toDoRepository.DeleteToDoItem(id, out originalToDoItem);
                _rabbitMqClient.SendMessage("ToDoItem", new ToDoRabbitMessage()
                {
                    Action = ToDoRabbitActions.Delete,
                    OldItem = originalToDoItem,
                    NewItem = null,
                });
                return Ok();
            }
            catch (ToDoItemNotFountException)
            {
                return NotFound(new { message = $"ToDo item #{id} not found." });
            }
        }
    }
}
