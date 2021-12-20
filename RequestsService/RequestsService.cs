using DatabaseManager.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace RequestsServices
{
    public class RequestsService
    {
        private bool _isLogged = false;
        private string _token;

        public string PostUrl { get; set; } = string.Empty;

        public void Login(string username, string password)
        {
            HttpClient client = new();
            StringContent sc = new("{sending}", Encoding.UTF8, "application/json");
            var response = client.PostAsync(PostUrl, sc).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
                Console.WriteLine("WORK");

            _isLogged = true;
        }

        public void Logout()
        {
            _isLogged = false;
        }

        public bool Send(List<Announcement> announcements)
        {
            if (PostUrl == string.Empty || !_isLogged) return false;

            JsonSerializerSettings options = new() { NullValueHandling = NullValueHandling.Ignore };
            options.Formatting = Formatting.Indented;
            options.ContractResolver = new CamelCasePropertyNamesContractResolver();

            string result = JsonConvert.SerializeObject(announcements, options);
            Console.WriteLine(result);

            StringContent sc = new(result.ToString(), Encoding.UTF8, "application/json");
            HttpClient client = new();
            HttpResponseMessage response = client.PostAsync(PostUrl, sc).Result;

            string responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                Console.WriteLine("WORK!");

            Console.WriteLine(responseContent);
            return true;
        }
    }
}
 