using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Models;

namespace Tasks.Domain
{
    public interface IToDoRepository
    {
        public IList<ToDoItem> GetToDoItems();
        public ToDoItem CreateToDoItem(CreateToDoItemDto createToDoItemDto, string userName, string accountId);
        public ToDoItem UpdateToDoItem(CreateToDoItemDto createToDoItemDto, string userName, int id, out ToDoItem originalToDoItem);
        public void DeleteToDoItem(int id, out ToDoItem originalToDoItem);
        public ToDoItem FixDates(ToDoItem toDoItem);
    }
}
