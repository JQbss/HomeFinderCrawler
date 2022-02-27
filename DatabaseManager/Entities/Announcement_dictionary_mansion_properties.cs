using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Entities
{
    // TODO: Na podstawie tego jestem w stanie określić czego dotyczy dany synonim
    [Index(nameof(Name), IsUnique = true)]
    public class Announcement_dictionary_mansion_properties
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
