using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Announcement_dictionary_item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
