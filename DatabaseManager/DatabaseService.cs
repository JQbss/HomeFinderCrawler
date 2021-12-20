using DatabaseManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager
{
    public class DatabaseService
    {
        private readonly DataContext _dbContext;

        public DatabaseService(DataContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        //
        // SELECT by id
        //
        public Image GetImageById(int id)
        {
            return _dbContext.Images.Where(x => x.Id == id).FirstOrDefault();
        }

        public Announcement GetAnnouncementById(int id)
        {
            return _dbContext.Announcements.Where(x =>x.Id == id).FirstOrDefault();
        }

        public Announcements_dictionary_status GetAnnouncementsDictionaryStatus(int id)
        {
            return _dbContext.AnnouncementsDictionaryStatuses.Where(x => x.Id == id).FirstOrDefault();
        }

        public Crawler_website GetCrawlerWebsiteById(int id)
        {
            return _dbContext.CrawlerWebsites.Where(x => x.Id == id).FirstOrDefault();
        }

        //
        // SELECT by link
        //

        public Announcement GetAnnouncementByLink(string link)
        {
            return _dbContext.Announcements.Include(x => x.Images).Where(x => x.Link == link).FirstOrDefault();
        }

        //
        // SELECT by sent
        //
        public List<Announcement> GetAnnouncementsBySentAndProcessed(bool sent, bool processed = true)
        {
            return _dbContext.Announcements.Include(x => x.Images).Where(x => x.Sent == sent && x.Processed == processed).ToList();
        }

        //SELECT * FROM TABLE
        public List<Crawler_website> GetCrawlerWebsites()
        {
            return _dbContext.CrawlerWebsites.Include(x => x.Crawler_Announcement).ToList();
        }

        public List<Announcement> GetAnnouncements()
        {
            return _dbContext.Announcements.Include(x => x.Images).ToList();
        }


        //SELECT BY URL
        public Crawler_website GetCrawlerWebsiteByUrl(string url)
        {
            return _dbContext.CrawlerWebsites.Include(x => x.Crawler_Announcement).Where(x => x.Website == url).FirstOrDefault();
        }

        //
        // INSERT
        //
        public bool AddImage(Image image)
        {
            if (image is null) return false;

            _dbContext.Images.Add(image);
            return true;
        }

        public bool AddAnnouncement(Announcement announcement)
        {
            if(announcement is null) return false;

            _dbContext.Announcements.Add(announcement);
            return true;
        }

        public bool AddAnnouncementDictionaryStatus(Announcements_dictionary_status announcementsDictionaryStatus)
        {
            if (announcementsDictionaryStatus is null) return false;

            _dbContext.AnnouncementsDictionaryStatuses.Add(announcementsDictionaryStatus);
            return true;
        }

        public bool AddCrawlerWebsite(Crawler_website crawler_Website)
        {
            if (crawler_Website is null) return false;

            _dbContext.CrawlerWebsites.Add(crawler_Website);
            return true;
        }

        //DELETE

        //TODO
        public bool RemoveCrawlerWebsite(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            //_dbContext.CrawlerWebsites.Dele;
            return true;
        }


        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
