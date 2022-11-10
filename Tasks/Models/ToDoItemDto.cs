namespace Tasks.Models
{
    public class ToDoItemDto : CreateToDoItemDto
    {
        public ToDoItemDto(ToDoItem toDoItem) : base(toDoItem)
        {
            Id = toDoItem.Id;
            AccountId = toDoItem.AccountId;
            CreatedAt = DateTime.SpecifyKind(toDoItem.CreatedAt, DateTimeKind.Utc);
            ModifedOn = toDoItem.ModifedOn != null ? DateTime.SpecifyKind((DateTime)toDoItem.ModifedOn, DateTimeKind.Utc) : null;
            CreatedBy = toDoItem.CreatedBy;
            ModifiedBy = toDoItem.ModifiedBy;
        }

        public int Id { get; set; }
        public string AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifedOn { get; set; }
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
