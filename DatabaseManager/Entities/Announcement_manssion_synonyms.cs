using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Announcement_manssion_synonyms
    {
        public int Id { get; set; }
        public string Value { get; set; }

        [ForeignKey("CrawlerWebsiteId")]
        public virtual Crawler_website Crawler_Website { get; set; }

        [ForeignKey("AnnouncementDictionaryManssionPropertiesId")]
        public virtual Announcement_dictionary_mansion_properties Announcement_dictionary_mansion_properties { get; set; }
    }
}
