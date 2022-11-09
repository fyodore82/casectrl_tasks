namespace Tasks.Models
{
    public class CreateToDoItemDto
    {
        public CreateToDoItemDto() { }
        public CreateToDoItemDto(ToDoItem toDoItem)
        {
            Title = toDoItem.Title;
            Description = toDoItem.Description;
        }

        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
