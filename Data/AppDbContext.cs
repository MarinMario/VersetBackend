using Microsoft.EntityFrameworkCore;
using VersuriAPI.Models.Entities;

namespace VersuriAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
    }
}
