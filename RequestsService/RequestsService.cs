using DatabaseManager.Entities;
using Newtonsoft.Json;

namespace RequestsServices
{
    public class RequestsService
    {
        public bool Send(List<Announcement> announcements)
        {
            var options = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            options.Formatting = Formatting.Indented;

            var result = JsonConvert.SerializeObject(announcements, options);
            Console.WriteLine(result.ToString());

            return true;
        }
    }
}
