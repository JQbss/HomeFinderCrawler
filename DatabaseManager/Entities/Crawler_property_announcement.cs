using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    // Klasa odpowidzialna za ogłoszenia związane typowo z nieruchomościami
    public class Crawler_property_announcement
    {
        public int Id { get; set; }

        [ForeignKey("CrawlerAnnouncementId")]
        public virtual Crawler_announcement Crawler_Announcement { get; set; }
    }
}
