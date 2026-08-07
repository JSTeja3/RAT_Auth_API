using Microsoft.EntityFrameworkCore;
using RAT_AUTH_API.Models;

namespace RAT_AUTH_API.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        public DbSet<User> Users{get; set;}
    }
    
}