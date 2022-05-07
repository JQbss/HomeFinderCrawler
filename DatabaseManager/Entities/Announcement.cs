using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DatabaseManager.Entities
{
    [Index(nameof(Link), IsUnique = true)]

    public class Announcement : IEquatable<Announcement>
    {
        //Primary Key
        [JsonIgnore]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int? Price { get; set; }

        public string Link { get; set; }
        // home, apartment, garage... ... Bike
        // TODO: To tutaj tak nie może działać -- na razie zrobić hardcode z typem ogłoszenia
        public Announcement_dictionary_item? Announcements_dictionary_item { get; set; }

        //TODO: na razie jest tylko = "manssion"
        [JsonProperty("type")]
        public string? Announcement_type { get; set; }

        //sale, rent ...
        public Announcement_dictionary_category? Announcements_dictionary_category { get; set; }

        // active, expired, deleted
        public Announcements_dictionary_status? Announcements_dictionary_status { get; set; }

        [JsonProperty("priceIsNegotiable")]
        public bool? To_negotiate { get; set; }

        //TODO: Localization table
        //public int Localization_dictionary_id { get; set; }
        [JsonIgnore]
        public ICollection<Image>? Images { get; set; }
        
        [JsonIgnore]
        public bool Processed { get; set; }

        [JsonIgnore]
        public Crawler_website Crawler_Website { get; set; }
        
        [JsonIgnore]
        public bool Sent { get; set; } = false;

        //TODO: Takie akcje nie powinny być wykonywane na encjach
        public bool Equals(Announcement? other)
        {
            if(other is null) return false;

            if(ReferenceEquals(this, other)) return true;

            return Link.Equals(other.Link);
        }

        public override int GetHashCode()
        {
            int hashDescription = Description != null ? Description.GetHashCode() : 0;
            int hashLink = Link != null ? Link.GetHashCode() : 0;

            return hashDescription ^ hashLink;
        }
    }
}
