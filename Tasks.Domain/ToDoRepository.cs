using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Models;

namespace Tasks.Domain
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly AppDbContext _context;
        public ToDoRepository(AppDbContext context)
        {
            _context = context;
        }

        public IList<ToDoItem> GetToDoItems()
        {
            return _context.ToDoItems.Select(FixDates).ToList();
        }

        public ToDoItem CreateToDoItem(CreateToDoItemDto createToDoItemDto, string userName, string accountId)
        {
            var toDoItem = new ToDoItem()
            {
                Id = 0,
                Title = createToDoItemDto.Title,
                Description = createToDoItemDto.Description,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = userName,
                ModifedOn = null,
                ModifiedBy = null,
                AccountId = accountId,
            };
            _context.ToDoItems.Add(toDoItem);
            _context.SaveChanges();
            return toDoItem;
        }

        public ToDoItem UpdateToDoItem(CreateToDoItemDto createToDoItemDto, string userName, int id, out ToDoItem originalToDoItem)
        {
            var toDoItem = _context.ToDoItems.SingleOrDefault(item => item.Id == id);

            if (toDoItem == null) throw new ToDoItemNotFountException();

            toDoItem = FixDates(toDoItem);
            originalToDoItem = new ToDoItem(toDoItem);

            toDoItem.Title = createToDoItemDto.Title;
            toDoItem.Description = createToDoItemDto.Description;
            toDoItem.ModifedOn = DateTime.Now.ToUniversalTime();
            toDoItem.ModifiedBy = userName;
            _context.SaveChanges();
            return toDoItem;
        }

        public void DeleteToDoItem(int id, out ToDoItem originalToDoItem)
        {
            var toDoItem = _context.ToDoItems.SingleOrDefault(item => item.Id == id);
            if (toDoItem == null) throw new ToDoItemNotFountException();
            originalToDoItem = new ToDoItem(FixDates(toDoItem));
            _context.Remove(toDoItem);
            _context.SaveChanges();
        }

        public ToDoItem FixDates(ToDoItem toDoItem)
        {
            toDoItem.CreatedAt = DateTime.SpecifyKind(toDoItem.CreatedAt, DateTimeKind.Utc);
            if (toDoItem.ModifedOn != null)
            {
                toDoItem.ModifedOn = DateTime.SpecifyKind((DateTime)toDoItem.ModifedOn, DateTimeKind.Utc);
            }
            return toDoItem;
        }
    }
}
