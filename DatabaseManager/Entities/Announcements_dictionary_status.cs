using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Announcements_dictionary_status
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
