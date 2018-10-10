namespace IRunes.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class IRunesContext : DbContext
    {
        public IRunesContext(DbContextOptions options) 
            : base(options) { }

        public IRunesContext() { }

        public DbSet<User> Users { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }
    }
}
