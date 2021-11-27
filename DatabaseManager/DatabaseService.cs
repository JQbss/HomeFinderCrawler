using DatabaseManager.Entities;

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
            return _dbContext.Announcements.Where(x => x.link == link).FirstOrDefault();
        }

        //SELECT * FROM TABLE
        public List<Crawler_website> GetCrawlerWebsites()
        {
            return _dbContext.CrawlerWebsites.ToList();
        }

        public List<Announcement> GetAnnouncements()
        {
            return _dbContext.Announcements.ToList();
        }

        //
        // INSERT
        //
        public bool AddImage(Image image)
        {
            if (image == null) return false;

            _dbContext.Images.Add(image);
            return true;
        }

        public bool AddAnnouncement(Announcement announcement)
        {
            if(announcement == null) return false;

            _dbContext.Announcements.Add(announcement);
            return true;
        }

        public bool AddAnnouncementDictionaryStatus(Announcements_dictionary_status announcementsDictionaryStatus)
        {
            if (announcementsDictionaryStatus == null) return false;

            _dbContext.AnnouncementsDictionaryStatuses.Add(announcementsDictionaryStatus);
            return true;
        }

        public bool AddCrawlerWebsite(Crawler_website crawler_Website)
        {
            if (crawler_Website == null) return false;

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

        public void Test()
        {
            _dbContext.Database.EnsureCreated();

            _dbContext.Announcements.AddRange(new Announcement[] {

                    new Announcement(){
                    Id=1,
                    Description = "fajne ogłoszenie, a czynsz mały",
                    Images = new List<Image>{
                        new Image() { Id = 1, Url = "www.google.com" },
                        new Image() { Id = 2, Url = "www.bing.com" }
                        },
                    Title = "Tytuł",
                    Price = 1234,
                    To_negotiate = true,
                    Announcements_dictionary_item = new Announcement_dictionary_item() { Name = "Sprzedaż"},
                    Announcements_dictionary_category = new Announcement_dictionary_category() { Name = "Sprzedażż"},
                    Announcements_dictionary_status = new Announcements_dictionary_status() { Name = "Aktywnee"},
                    link = "www.olx.pl"
                    }
            });

            _dbContext.SaveChanges();

            _dbContext.Announcements?.ToList().ForEach(announcement =>
            {
                announcement.Images?.ForEach(image =>
                {
                    Console.WriteLine(image.Url);
                });
            });
        }
    }
}
