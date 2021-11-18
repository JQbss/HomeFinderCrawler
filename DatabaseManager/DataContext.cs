using DatabaseManager.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DatabaseManager
{
    public class DataContext : DbContext
    {
        private string _source = string.Empty;
        public DbSet<Image> Images { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

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
            modelBuilder.Entity<Image>().ToTable("Images", "test");
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id);
            });
            modelBuilder.Entity<Announcement>().ToTable("Announcements", "test"); 

            base.OnModelCreating(modelBuilder);
        }

    }
}


/*
    class DatabaseContext : DbContext
    {
        public DatabaseContext() :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "D:\\Databases\\SQLiteWithEF.db", ForeignKeys = true }.ConnectionString
            }, true)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<EmployeeMaster> EmployeeMaster { get; set; }
    }

*/