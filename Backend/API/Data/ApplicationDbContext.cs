
using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Data
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Admin> admins {  get; set; }
    }
}
