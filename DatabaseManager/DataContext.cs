using DatabaseManager.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DatabaseManager
{
    public class DataContext : DbContext
    {
        private readonly string _source = string.Empty;
        public DbSet<Image> Images { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Announcements_dictionary_status> AnnouncementsDictionaryStatuses { get; set; }

        public DataContext(string source)
        {
            _source = source;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_source, option =>
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO: Usless code
            modelBuilder.Entity<Image>().ToTable("Images", "test");
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id);
            });
            modelBuilder.Entity<Announcement>().ToTable("Announcements", "test");
            modelBuilder.Seed();

            base.OnModelCreating(modelBuilder);
        }

    }
}
