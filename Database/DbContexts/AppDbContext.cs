using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DbContexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<EmployeeDb> Employees { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
