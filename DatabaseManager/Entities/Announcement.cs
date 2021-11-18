using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{

    [Table("Announcement")]
    public class Announcement
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
