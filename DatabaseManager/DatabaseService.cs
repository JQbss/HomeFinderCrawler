using Microsoft.EntityFrameworkCore;
using DatabaseManager.Entities;

namespace DatabaseManager
{
    public class DatabaseService
    {
        private DataContext _dbContext;

        public DatabaseService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Test()
        {

            _dbContext.Database.EnsureCreated();

            _dbContext.Announcements.AddRange(new Entities.Announcement[] {

                    new DatabaseManager.Entities.Announcement(){ Id=1, Description = "www.google.com"}
            });

            _dbContext.SaveChanges();
            _dbContext.Announcements?.ToList().ForEach(announcement =>
            {
                Console.WriteLine(announcement.Description);
            });
        }
    }
}
