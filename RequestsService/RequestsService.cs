using DatabaseManager.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace RequestsServices
{
    public class RequestsService
    {
        // Hasło i login nie powinny być tutaj trzymane pod żadnym pozorem
        // To jest chyba za bardzo niebezpieczne
        private bool _isLogged = false;
        private string _token;

        public string PostUrl { get; set; } = string.Empty;

        public void Login(string username, string password)
        {
            HttpClient client = new();
            StringContent sc = new($"{{email:\"{username}\", password:\"{password}\"}}", Encoding.UTF8, "application/json");
            var response = client.PostAsync(PostUrl, sc).Result;
            response.Content.ReadAsStringAsync();

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Work");
                _isLogged = true;
            }
            //Sytuacja kiedy nie udało nam się zalogować
            else
            {
                _isLogged= false;
            }
        }

        public void Logout()
        {
            _isLogged = false;

            //Usuwanie tokena
            _token = string.Empty;
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

            response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("WORK!");
                return true;
            }
            return false;
        }
    }
}
