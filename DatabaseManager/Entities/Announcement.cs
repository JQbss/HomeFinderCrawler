using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Announcement
    {
        //Primary Key
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int? Price { get; set; }

        public string link { get; set; }
        // home, apartment, garage... ... Bike
        public Announcement_dictionary_item? Announcements_dictionary_item { get; set; }

        //sale, rent ...
        public Announcement_dictionary_category? Announcements_dictionary_category { get; set; }
        
        // active, expired, deleted
        public Announcements_dictionary_status? Announcements_dictionary_status { get; set; }

        public bool? To_negotiate { get; set; }

        //TODO: Localization table
        //public int Localization_dictionary_id { get; set; }
        public List<Image>? Images { get; set; }
        public bool Processed { get; set; }
    }
}
