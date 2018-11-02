using Microsoft.EntityFrameworkCore;
using MishMashWebApp.Models;

namespace MishMashWebApp.Data
{
    public class MishMashDbContext : DbContext
    {
        public MishMashDbContext()
        {
        }

        public MishMashDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ChannelTag> ChannelsTags { get; set; }

        public DbSet<UserChannel> UsersChannels { get; set; }

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
            modelBuilder.Entity<ChannelTag>()
                .HasKey(ct => new { ct.ChannelId, ct.TagId });

            modelBuilder.Entity<UserChannel>()
                .HasKey(uc => new { uc.ChannelId, uc.UserId });
        }
    }
}
