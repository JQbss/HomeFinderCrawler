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

        /// <summary>
        /// Returns the entity with the website
        /// </summary>
        /// <param name="web_name">Website URL</param>
        /// <returns>Crawler_website</returns>
        public Crawler_website? GetCrawlerAnnouncement(string web_name)
        {
            return _dbContext.CrawlerWebsites
                .Where(x => x.Website == web_name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns an entity from a dictionary of property properties
        /// </summary>
        /// <param name="property_name">Property name</param>
        /// <returns>Announcement_dictionary_mansion_properties</returns>
        public Announcement_dictionary_mansion_properties? GetAnnonuncementDictionaryMansionPropertiesId(string property_name)
        {
            return _dbContext.AnnouncementDictionaryMansionProperties
                .Where(x => x.Name == property_name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the entity with the announcement
        /// </summary>
        /// <param name="id">Announcement ID</param>
        /// <returns>Announcement class</returns>
        public Announcement? GetAnnouncementById(int id)
        {
            return _dbContext.Announcements
                .Where(x =>x.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// The function returns synonym array based on the synonym name
        /// </summary>
        /// <param name="Synonym_name">Property synonym name</param>
        /// <param name="WebSite_ID">Website ID</param>
        /// <returns></returns>
        public string[] GetAnnouncementsManssionSynonyms(string Synonym_name, int WebSite_ID)
        {
            return _dbContext.AnnouncementManssionSynonyms
                .Include(x => x.Announcement_dictionary_mansion_properties)
                .Where(x => x.Announcement_dictionary_mansion_properties.Name.Equals(Synonym_name) && x.Crawler_Website.Id == WebSite_ID)
                .Select(x => x.Value)
                .ToArray();
        }

        /// <summary>
        /// The function returns the id of the webpage from which the advertisement comes
        /// </summary>
        /// <param name="AnnouncementID"> Announcement ID</param>
        /// <returns>int with crawler website id</returns>
        public int GetAnnonucementCrawlerWebsiteId(int AnnouncementID)
        {
            return _dbContext.Announcements
                .Where(x => x.Id == AnnouncementID)
                .Include(x => x.Crawler_Website)
                .Select(x => x.Crawler_Website.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// The function returns an advertisement based on an advertisement link
        /// </summary>
        /// <param name="link"> announcement link</param>
        /// <returns>Announcement</returns>
        public Announcement? GetAnnouncementByLink(string link)
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .Where(x => x.Link == link)
                .FirstOrDefault();
        }

        /// <summary>
        /// Function that returns a list of real estate advertisements filtered after sent and processed
        /// </summary>
        /// <param name="sent">Or sent ads</param>
        /// <param name="processed">Or processed ads</param>
        /// <returns>List of real estate advertisment</returns>
        public List<Announcement_manssion> GetAnnouncementManssionBySendAndProcessed(bool sent, bool processed = true)
        {
            return _dbContext.AnnouncementManssions
                .Include(x => x.Announcement)
                .Include(x => x.Announcement.Images)
                .Where(x => x.Announcement.Sent == sent && x.Announcement.Processed == processed).ToList();
        }

        /// <summary>
        /// Function that returns a list of advertisements filtered after sent and processed
        /// </summary>
        /// <param name="sent">Or sent ads</param>
        /// <param name="processed">Or processed ads</param>
        /// <returns>Announcement list</returns>
        public List<Announcement> GetAnnouncementsBySentAndProcessed(bool sent, bool processed = true)
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .Where(x => x.Sent == sent && x.Processed == processed)
                .ToList();
        }

        /// <summary>
        /// The function returns a list of web pages processed by the crawler
        /// </summary>
        /// <returns>Websites in crawler list</returns>
        public List<Crawler_website> GetCrawlerWebsites()
        {
            return _dbContext.CrawlerWebsites
                .Include(x => x.Crawler_Announcement)
                .Include(x => x.Crawler_Announcement.Crawler_Website_Link_Contains)
                .ToList();
        }

        /// <summary>
        /// The function returns a list of all ads in the crawler
        /// </summary>
        /// <returns>Announcements list</returns>
        public List<Announcement> GetAnnouncements()
        {
            return _dbContext.Announcements
                .Include(x => x.Images)
                .ToList();
        }

        /// <summary>
        /// The function checks if a given synonym exists for a given page.
        /// </summary>
        /// <param name="CrawlerWebsiteID">Website ID</param>
        /// <param name="AnnouncementDictionaryMansionPropertiesId">Announcement Dictionary Mansion Properties id</param>
        /// <param name="SynonymValue"> Synonym Value</param>
        /// <returns>boolean information about exists</returns>
        public bool CheckSynonymExists(int CrawlerWebsiteID, int AnnouncementDictionaryMansionPropertiesId, string SynonymValue)
        {
            if (SynonymValue == null || SynonymValue.Length == 0)
                return false;

            var tmp = _dbContext.AnnouncementManssionSynonyms
                .Where(x => x.Value == SynonymValue && x.Crawler_Website.Id == CrawlerWebsiteID && x.Announcement_dictionary_mansion_properties.Id == AnnouncementDictionaryMansionPropertiesId)
                .FirstOrDefault();

            return tmp is not null;
        }

        /// <summary>
        /// The function returns information about a real estate advertisement related to a given advertisement.
        /// </summary>
        /// <param name="AnnouncemenetId"> Announcement Id</param>
        /// <returns>Announcement manssion</returns>
        public Announcement_manssion? GetAnnouncementManssionByAnnouncemenetId(int AnnouncemenetId)
        {
            return _dbContext.AnnouncementManssions
                .Where(x => x.Announcement.Id == AnnouncemenetId)
                .FirstOrDefault();
        }

        /// <summary>
        /// The function adds a real estate advertisement
        /// </summary>
        /// <param name="announcement_Manssion">announcement manssion to add</param>
        /// <returns>True if the ad has been added</returns>
        public bool AddAnnouncementManssion(Announcement_manssion announcement_Manssion)
        {
            if (announcement_Manssion is null) return false;

            _dbContext.AnnouncementManssions.Add(announcement_Manssion);
            return true;
        }

        /// <summary>
        /// The function adds announcement
        /// </summary>
        /// <param name="announcement">Announcement to add</param>
        /// <returns>True if the ad has been added</returns>
        public bool AddAnnouncement(Announcement announcement)
        {
            if(announcement is null) return false;

            _dbContext.Announcements.Add(announcement);
            return true;
        }

        /// <summary>
        /// The function adds announcement manssion synonym
        /// </summary>
        /// <param name="announcement_Manssion_Synonyms">Announcement Manssion Synonym</param>
        /// <returns>True if synonym has been added</returns>
        public bool AddSynonymPropertiesWebsite(Announcement_manssion_synonyms announcement_Manssion_Synonyms)
        {
            if (announcement_Manssion_Synonyms is null) return false;

            _dbContext.AnnouncementManssionSynonyms.Add(announcement_Manssion_Synonyms);
            return true;
        }

        /// <summary>
        /// The function adds crawler website
        /// </summary>
        /// <param name="crawler_Website">Website to add</param>
        /// <returns>True if the website has been added</returns>
        public bool AddCrawlerWebsite(Crawler_website crawler_Website)
        {
            if (crawler_Website is null) return false;

            _dbContext.CrawlerWebsites.Add(crawler_Website);
            return true;
        }

        /// <summary>
        /// The function commit changes to the database
        /// </summary>
        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
