using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Entities
{
    [Index(nameof(Website), IsUnique = true)] 
    public class Crawler_website
    {
        public int Id { get; set; }
        public string Website { get; set; }
        public string Regex { get; set; }
        public int RefreshTime { get; set; } = 1000;
        public int MaxPages { get; set; }
        public string Prelink { get; set; }
        public string Pagequery { get; set; } = "?page=";
        public Crawler_announcement Crawler_Announcement { get; set; }
    }
}
