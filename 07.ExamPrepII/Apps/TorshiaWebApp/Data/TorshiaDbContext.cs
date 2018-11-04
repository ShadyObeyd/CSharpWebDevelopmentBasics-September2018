using Microsoft.EntityFrameworkCore;
using TorshiaWebApp.Models;

namespace TorshiaWebApp.Data
{
    public class TorshiaDbContext : DbContext
    {
        public TorshiaDbContext(DbContextOptions options) : base(options)
        {
        }

        public TorshiaDbContext()
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskSector> TaskSectors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
