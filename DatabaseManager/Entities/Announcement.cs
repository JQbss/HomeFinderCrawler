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

        [JsonProperty("type")]
        [JsonIgnore]
        public string? Announcement_type { get; set; }

        [JsonProperty("priceIsNegotiable")]
        public bool? To_negotiate { get; set; }

        [JsonIgnore]
        public ICollection<Image>? Images { get; set; }
        
        [JsonIgnore]
        public bool Processed { get; set; }

        [JsonIgnore]
        public Crawler_website Crawler_Website { get; set; }
        
        [JsonIgnore]
        public bool Sent { get; set; } = false;

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
