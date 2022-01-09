namespace DatabaseManager.Entities
{
    // Ogólna klasa do zarządzania "wszystkimi" ogłoszeniami
    public class Crawler_announcement
    {
        public int Id { get; set; }
        public string Image_node_name { get; set; } = string.Empty;

        // Image
        public string Image_attribute_value { get; set; }
        public string Image_class_name { get; set; }

        //Title 
        public string Title_node_name { get;set; }
        
        //Price
        public string Price_node_name { get; set; }

        //Description
        public string Description_node_name { get; set; }
        public string Description_class_name { get; set; }

        public ICollection<Crawler_announcements_link_contains>? Crawler_Website_Link_Contains { get; set; }
    }
}
