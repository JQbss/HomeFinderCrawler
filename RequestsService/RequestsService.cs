using DatabaseManager.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace RequestsServices
{
    public class RequestsService
    {
        // Service using in logging errror
        IErrorLogService? _logService = null;
        private bool _isLogged = false;
        private string _token = string.Empty;
        private const string _applicationModuleName = "[RequestsService]";
        private HttpClient _httpClient = new();

        public string PostUrl { get; set; } = string.Empty;

        public string UsersURL { get; set; } = string.Empty;
        public string AnnouncementsUrl { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
        public string RegisterUrl { get; set; } = string.Empty;

        public void AddErrorLogService(IErrorLogService logService) => _logService = logService;
        public void StartErrorLogService() => _logService?.StartLog();
        public void StopErrorLogService() => _logService?.StopLog();

        public void Register(string username, string password)
        {
            if (RegisterUrl == String.Empty)
            {
                _logService?.AddLog("Register failed. Username:" + username + " RegisterUrl:" + RegisterUrl);
                return;
            }

            JObject json = new();
            json.Add("email", username);
            json.Add("password", password);

            Console.WriteLine("Register.json");
            Console.WriteLine(json.ToString());

            StringContent sc = new(json.ToString(), Encoding.UTF8, "application/json");

            try
            {
                var response = _httpClient.PostAsync(RegisterUrl, sc).Result;

                var content = response.Content.ReadAsStringAsync();

                if (response.StatusCode is System.Net.HttpStatusCode.OK)
                    _logService?.AddLog(_applicationModuleName + "Register user:" + username + " successful");
                else
                    _logService?.AddLog(_applicationModuleName + "Register user:" + username + " failed. Content:" + content.Result.ToString());
            }
            catch (Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Register user:" + username + " failed. Exception:" + e.Message);
            }
        }

        public void Login(string username, string password)
        {
            if(LoginUrl == string.Empty)
            {
                _logService?.AddLog(_applicationModuleName + "Login failed. Username:" + username + " LoginUrl:" + LoginUrl);
                return;
            }


            JObject json = new();
            json.Add("email", username);
            json.Add("password", password);

            //Console.WriteLine("Login.json");
            //Console.WriteLine(json.ToString());

            StringContent sc = new(json.ToString(), Encoding.UTF8, "application/json");
            try
            {
                var response = _httpClient.PostAsync(LoginUrl, sc).Result;
                var content = response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Login result");
                    Console.WriteLine(content.Result.ToString());
                    json = JObject.Parse(content.Result.ToString());
                    if (json.GetValue("idToken") is not null)
                    {
                        _token = json.GetValue("idToken").ToString();
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                        _isLogged = true;
                        _logService?.AddLog(_applicationModuleName + "Logged to api successful");
                    }
                }
                else
                {
                    _isLogged = false;
                    _logService?.AddLog(_applicationModuleName + "Login failed. Username:" + username + " Content:" + content.Result.ToString());
                }
            }
            catch (Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Login failed. Username:" + username + " Exception:" + e.Message);
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
            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(PostUrl, sc).Result;

                response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("WORK!");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Exception:" + e.Message);
                return false;
            }
        }

        public bool AddAnnouncements(List<Announcement> announcements)
        {
            if (AnnouncementsUrl == string.Empty && _isLogged)
            {
                _logService?.AddLog(_applicationModuleName + "Error in adding Announcement. AnnouncementsUrl: " + AnnouncementsUrl + "_isLogged: " + _isLogged);
                return false;
            }

            foreach(Announcement announcement in announcements)
                AddAnnouncement(announcement);

            return true;
        }

        public bool AddAnnouncement(Announcement announcement)
        {
            // TODO: Better parsing json.
            JObject json = new();
            json.Add("description", announcement.Description);
            json.Add("title", announcement.Title);
            json.Add("link", announcement.Link);

            StringContent sc = new(json.ToString(), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(AnnouncementsUrl, sc).Result;
                var content = response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    _logService?.AddLog(_applicationModuleName + "Succesfull send announcement id:" + announcement.Id);
                    announcement.Sent = true;
                    return true;
                }
                else
                {
                    _logService?.AddLog(_applicationModuleName + "Failed send announcement id:" + announcement.Id + " Content:" + content.Result.ToString());
                    return false;
                }
            }
            catch(Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Exception announcement id:" + announcement.Id + " Exception:" + e.Message);
                return false;
            }
        }

        ~RequestsService()
        {
            _httpClient?.Dispose();
        }
    }
}
