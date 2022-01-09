using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public  class Crawler_announcements_link_contains
    {
        public int Id { get; set; }

        public string Value { get; set; }
        public bool IsContains { get; set; } = true;

        [ForeignKey("CrawlerAnnouncementId")]
        public virtual Crawler_announcement Crawler_Announcement { get; set; }
    }
}
