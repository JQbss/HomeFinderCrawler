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

        public Crawler_website? GetCrawlerAnnouncementId(string web_name)
        {
            return _dbContext.CrawlerWebsites
                .Where(x => x.Website == web_name)
                .FirstOrDefault();
        }
        public Announcement_dictionary_mansion_properties? GetAnnonuncementDictionaryMansionPropertiesId(string property_name)
        {
            return _dbContext.AnnouncementDictionaryMansionProperties
                .Where(x => x.Name == property_name)
                .FirstOrDefault();
        }

        //
        // SELECT by id
        //
        public Image? GetImageById(int id)
        {
            return _dbContext.Images
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public Announcement? GetAnnouncementById(int id)
        {
            return _dbContext.Announcements
                .Where(x =>x.Id == id)
                .FirstOrDefault();
        }

        public Announcements_dictionary_status? GetAnnouncementsDictionaryStatus(int id)
        {
            return _dbContext.AnnouncementsDictionaryStatuses
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public Crawler_website? GetCrawlerWebsiteById(int id)
        {
            return _dbContext.CrawlerWebsites
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Funkcja zwracąca synonimy
        /// </summary>
        /// <param name="Synonym_name">Wartość właściwości</param>
        /// <returns>Zwraca tablicę z synonimamy dla danych właściwości nieruchomości</returns>
        public string[] GetAnnouncementsManssionSynonyms(string Synonym_name, int WebSite_ID)
        {
            return _dbContext.AnnouncementManssionSynonyms
                .Include(x => x.Announcement_dictionary_mansion_properties)
                .Where(x => x.Announcement_dictionary_mansion_properties.Name.Equals(Synonym_name) && x.Crawler_Website.Id == WebSite_ID)
                .Select(x => x.Value)
                .ToArray();
        }

        public int GetAnnonucementCrawlerWebsiteId(int AnnouncementID)
        {
            return _dbContext.Announcements
                .Where(x => x.Id == AnnouncementID)
                .Include(x => x.Crawler_Website)
                .Select(x => x.Crawler_Website.Id)
                .FirstOrDefault();
        }
        //
        // SELECT by link
        //

        public Announcement? GetAnnouncementByLink(string link)
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .Where(x => x.Link == link)
                .FirstOrDefault();
        }

        //
        // SELECT by sent
        //
        public List<Announcement> GetAnnouncementsBySentAndProcessed(bool sent, bool processed = true)
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .Where(x => x.Sent == sent && x.Processed == processed)
                .ToList();
        }

        //SELECT * FROM TABLE
        public List<Crawler_website> GetCrawlerWebsites()
        {
            return _dbContext.CrawlerWebsites
                .Include(x => x.Crawler_Announcement)
                .Include(x => x.Crawler_Announcement.Crawler_Website_Link_Contains)
                .ToList();
        }

        public List<Announcement> GetAnnouncements()
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .ToList();
        }


        //SELECT BY URL
        public Crawler_website? GetCrawlerWebsiteByUrl(string url)
        {
            return _dbContext.CrawlerWebsites
                .Include(x => x.Crawler_Announcement)
                .Where(x => x.Website == url)
                .FirstOrDefault();
        }

        // Funkcja sprawdzająca czy istnieje już podany synonim.
        public bool CheckSynonymExists(int CrawlerWebsiteID, int AnnouncementDictionaryMansionPropertiesId, string SynonymValue)
        {
            if (SynonymValue == null || SynonymValue.Length == 0)
                return false;

            // Sprawdznie czy synonim istnieje
            var tmp = _dbContext.AnnouncementManssionSynonyms
                .Where(x => x.Value == SynonymValue && x.Crawler_Website.Id == CrawlerWebsiteID && x.Announcement_dictionary_mansion_properties.Id == AnnouncementDictionaryMansionPropertiesId)
                .FirstOrDefault();

            return tmp is not null ? true : false;
        }

        //SELECT * FROM ANNOUNCEMENT MANSSION BY ANNOUNCEMENT ID
        public Announcement_manssion? GetAnnouncementManssionByAnnouncemenetId(int AnnouncemenetId)
        {
            return _dbContext.AnnouncementManssions
                .Where(x => x.Announcement.Id == AnnouncemenetId)
                .FirstOrDefault();
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

        public bool AddAnnouncementManssion(Announcement_manssion announcement_Manssion)
        {
            if (announcement_Manssion is null) return false;

            _dbContext.AnnouncementManssions.Add(announcement_Manssion);
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

        public bool AddSynonymPropertiesWebsite(Announcement_manssion_synonyms announcement_Manssion_Synonyms)
        {
            if (announcement_Manssion_Synonyms is null) return false;

            _dbContext.AnnouncementManssionSynonyms.Add(announcement_Manssion_Synonyms);
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
