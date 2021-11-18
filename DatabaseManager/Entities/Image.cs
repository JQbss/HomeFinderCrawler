using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    [Table("Image")]
    public class Image
    {
        public int Id { get; set; }
        public int Announcement_id { get; set; }
        public string Url { get; set; }
    }
}
