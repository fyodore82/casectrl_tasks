using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tasks.Models;

namespace Tasks.Domain
{
    public class AppDbContext : DbContext
    {
        private readonly AppSettings _appSettings;
        public AppDbContext(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public DbSet<ToDoItem> ToDoItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(_appSettings.DatabaseConnection ?? "");
    }
}
