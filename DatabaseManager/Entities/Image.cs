using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Image
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Url { get; set; }
        [JsonIgnore]
        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
    }
}
