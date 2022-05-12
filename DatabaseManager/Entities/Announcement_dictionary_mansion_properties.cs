using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Announcement_dictionary_mansion_properties
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
