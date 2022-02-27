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
        public DbSet<Crawler_website> CrawlerWebsites { get; set; }
        public DbSet<Announcement_manssion> AnnouncementManssions { get; set; }
        public DbSet<Announcement_dictionary_mansion_properties> AnnouncementDictionaryMansionProperties { get; set; }
        public DbSet<Announcement_manssion_synonyms> AnnouncementManssionSynonyms { get; set; }

        public DataContext(string source) => _source = source;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_source, option =>
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
    }
}
