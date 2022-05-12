using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Announcement_manssion
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("numberOfRooms")]
        [JsonIgnore]
        public string? Room_count { get; set; }

        [JsonIgnore]
        public string? Level { get; set; }

        public bool? Furnished { get; set; }

        [JsonProperty("typeOfBuilding")]
        [JsonIgnore]
        public string? Type_of_building { get; set; }

        public int? Rent_price { get; set; }

        public double? Area { get; set; }

        [JsonProperty("yearOfBuilding")]
        public int? Year_od_construction { get; set; }

        [JsonIgnore]
        public string? Localization { get; set; }

        [JsonIgnore]
        public string? Volume { get; set; }

        [JsonProperty("additionalArea")]
        public string? Additional_area { get; set; }

        [JsonIgnore]
        public string? Price_per_m2 { get; set; }

        [JsonIgnore]
        public string? Land_area { get; set; }

        [JsonIgnore]
        public string? Driveway { get; set; }

        [JsonIgnore]
        public string? State { get; set; }

        [JsonIgnore]
        public string? Heating_and_energy { get; set; }

        [JsonIgnore]
        public string? Media { get; set; }

        [JsonIgnore]
        public string? Fence_of_the_plot { get; set; }

        [JsonIgnore]
        public string? Shape_of_the_plot { get; set; }

        [JsonIgnore]
        public string? Apperance { get; set; }

        [JsonIgnore]
        public string? Number_of_positions { get; set; }

        [JsonIgnore]
        public string? Building_material { get; set; }

        [JsonIgnore]
        public bool Air_conditioning { get; set; }

        [JsonIgnore]
        public bool Balcony { get; set; }

        [JsonIgnore]
        public bool Basement { get; set; }

        [JsonIgnore]
        public bool Garage { get; set; }

        [JsonIgnore]
        public bool Garden { get; set; }

        [JsonIgnore]
        public bool Lift { get; set; }

        [JsonIgnore]
        public bool Non_smoking_only { get; set; }

        [JsonIgnore]
        public bool Separate_kitchen { get; set; }

        [JsonIgnore]
        public bool Terrace { get; set; }

        [JsonIgnore]
        public bool Two_storeys { get; set; }

        [JsonIgnore]
        public bool Utility_room { get; set; }

        [JsonIgnore]
        public bool Asphalt_access { get; set; }

        [JsonIgnore]
        public bool Heating { get; set; }

        [JsonIgnore]
        public bool Parking { get; set; }

        [JsonIgnore]
        public bool Site { get; set; }

        [JsonIgnore]
        public string? Type_of_roof { get; set; }

        [JsonIgnore]
        public bool Bungalow { get; set; }

        [JsonIgnore]
        public bool Recreational { get; set; }

        public string? Investment_status { get; set; }

        [JsonIgnore]
        public bool Internet { get; set; }

        [JsonIgnore]
        public bool Cable_TV { get; set; }

        [JsonIgnore]
        public bool Phone { get; set; }

        public string? Market { get; set; }

        [ForeignKey("AnnouncementId")]
        [JsonIgnore]
        public virtual Announcement Announcement { get; set; }
    }
}
