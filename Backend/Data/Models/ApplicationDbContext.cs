

using Microsoft.EntityFrameworkCore;
namespace Data.Models
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Admin> admins {  get; set; }
    }
}
