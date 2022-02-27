using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Announcement_manssion_synonyms
    {
        public int Id { get; set; }
        public string Value { get; set; }

        [ForeignKey("CrawlerAnnouncementId")]
        public virtual Crawler_announcement Crawler_Announcement { get; set; }

        [ForeignKey("AnnouncementDictionaryManssionProperties")]
        public virtual Announcement_dictionary_mansion_properties Announcement_dictionary_mansion_properties { get; set; }
    }
}
