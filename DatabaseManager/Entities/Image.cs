using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
