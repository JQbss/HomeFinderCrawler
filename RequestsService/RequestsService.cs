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

        /// <summary>
        /// A function that adds a service for logging errors in requests
        /// </summary>
        /// <param name="logService">IErrorLogService</param>
        public void AddErrorLogService(IErrorLogService logService) => _logService = logService;

        /// <summary>
        /// Function that starts error logging
        /// </summary>
        public void StartErrorLogService() => _logService?.StartLog();

        /// <summary>
        /// Function that stop error logging
        /// </summary>
        public void StopErrorLogService() => _logService?.StopLog();

        /// <summary>
        /// Function for registering a new user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
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

        /// <summary>
        /// Function for logging in with API
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
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

        /// <summary>
        /// Function for logging out of the API
        /// </summary>
        public void Logout()
        {
            _isLogged = false;

            //Usuwanie tokena
            _token = string.Empty;
        }

        // Przeciążenie funkcji
        /// <summary>
        /// Function for sending real estate advertisements
        /// </summary>
        /// <param name="announcement_Manssions"> List of Manssion announcements</param>
        /// <returns></returns>
        public bool Send(List<Announcement_manssion> announcement_Manssions)
        {
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
                    Console.WriteLine(result);
                    result.Merge(JObject.Parse(JsonConvert.SerializeObject(addMann.Announcement, options)));

                    // Creating address to json
                    // This should be in database
                    if (addMann.Localization is not null)
                    {
                        string[] address = addMann.Announcement.Localization.Split(',');

                        if (address is not null)
                        {
                            var add =
                            new
                            {
                                address =
                                new
                                {
                                    miejscowosc = address[0] is not null? address[0]: null,
                                    powiat = address[1] is not null? address[1]: null,
                                    wojewodztwo = address[2] is not null ? address[2] : null
                                }
                            };
                            result.Merge(JObject.Parse(JsonConvert.SerializeObject(add, options).ToString()));
                        }
                    }

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
            Console.WriteLine(JsonConvert.SerializeObject(oblist, options).ToString());
            return SendJson(JsonConvert.SerializeObject(oblist, options).ToString());
        }

        /// <summary>
        /// Private function to send Json to API
        /// </summary>
        /// <param name="json">string with json</param>
        /// <returns>True if request siccessfully sent</returns>
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
    }
}
