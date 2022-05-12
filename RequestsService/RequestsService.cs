using DatabaseManager.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace RequestsServices
{
    public class RequestsService
    {
        IErrorLogService? _logService = null;
        private bool _isLogged = false;
        private string _token = string.Empty;
        private const string _applicationModuleName = "[RequestsService]";
        private HttpClient _httpClient = new();

        public string PostUrl { get; set; } = string.Empty;
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

            JObject json = new()
            {
                { "email", username },
                { "password", password }
            };

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

            JObject json = new()
            {
                { "email", username },
                { "password", password }
            };

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

        // Przeciążenie funkcji
        public bool Send(List<Announcement_manssion> announcement_Manssions)
        {
            // Tutaj powinno być tylko przygotowanie pliku
            if (PostUrl == String.Empty || announcement_Manssions is null || announcement_Manssions.Count is 0) return false;

            JsonSerializerSettings options = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            List<JObject> oblist = new();

            // Tylko serializacja ogłoszenia
            foreach( Announcement_manssion addMann in announcement_Manssions)
            {
                if (addMann is not null)
                {
                    var result = JObject.Parse(JsonConvert.SerializeObject(addMann, options));
                    result.Merge(JObject.Parse(JsonConvert.SerializeObject(addMann.Announcement, options)));

                    // Creating address to json
                    // This should be in database
                    if (addMann.Localization is not null)
                    {
                        string[] address = addMann.Localization.Split(',');

                        if (address is not null && address.Length > 2)
                        {
                            var add =
                            new
                            {
                                address =
                                new
                                {
                                    miejscowosc = address[0],
                                    powiat = address[1],
                                    wojewodztwo = address[2]
                                }
                            };
                            result.Merge(JObject.Parse(JsonConvert.SerializeObject(add, options).ToString()));
                        }
                    }


                    // Tworzenie listy obrazów
                    // To jest mało wydajne, bo robię listę przez Add
                    List<string> imagesList = new();
                    if (imagesList is not null)
                    {
                        foreach (var img in addMann.Announcement.Images)
                            imagesList.Add(img.Url);

                        result.Merge(JObject.Parse(("{imageLinks: " + JsonConvert.SerializeObject(imagesList, options).ToString() + "}")));
                    }
                    oblist.Add(result);
                }
            }
            SendJson(JsonConvert.SerializeObject(oblist, options).ToString());
            Console.WriteLine(JsonConvert.SerializeObject(oblist, options).ToString());
            return true;
        }

        private bool SendJson(string json)
        {
            StringContent sc = new(json, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(PostUrl, sc).Result;

                response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;

                _logService?.AddLog(_applicationModuleName + "PostStatusCode:" + response.StatusCode);
                return false;
            }
            catch (Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Exception:" + e.Message);
                return false;
            }
        }


        public bool Send(List<Announcement> announcements)
        {
            if (PostUrl == string.Empty /*|| !_isLogged*/) return false;

            JsonSerializerSettings options = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string result = JsonConvert.SerializeObject(announcements, options);
            Console.WriteLine(result);

            StringContent sc = new(result.ToString(), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(PostUrl, sc).Result;

                response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
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
            JObject json = new()
            {
                { "description", announcement.Description },
                { "title", announcement.Title },
                { "link", announcement.Link }
            };
            StringContent sc = new(json.ToString(), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(AnnouncementsUrl, sc).Result;
                var content = response.Content.ReadAsStringAsync();

                Console.WriteLine(response.StatusCode);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    _logService?.AddLog(_applicationModuleName + "Succesfull send announcement id:" + announcement.Id);
                    announcement.Sent = true;
                    return true;
                }
                _logService?.AddLog(_applicationModuleName + "Failed send announcement id:" + announcement.Id + " Content:" + content.Result.ToString());
                return false;
            }
            catch(Exception e)
            {
                _logService?.AddLog(_applicationModuleName + "Exception announcement id:" + announcement.Id + " Exception:" + e.Message);
                return false;
            }
        }

        ~RequestsService() => _httpClient?.Dispose();
    }
}
