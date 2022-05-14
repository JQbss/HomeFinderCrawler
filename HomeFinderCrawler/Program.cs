using Crawler;
using DatabaseManager;
using DatabaseManager.Entities;
using RequestsServices;

namespace HomeFinderCrawler
{
    public class Program
    {
        // Global declaration
        private static WebCrawler _webCrawler;
        private static RequestsService _requestsService;
        private static System.Timers.Timer _crawlerTimer = new();

        // Running Crawler flag
        private static bool _isRunning = false;

        public static void Main(string[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            CleanCrawlerSimulation();
        }

        /// <summary>
        /// A function for testing the crawler's performance
        /// </summary>
        private static void TestCrawlerMethod()
        {
            WebCrawler cr = new(new DataContext("DataSource=craw.db"));

            cr.AddWebsite(AddOLXWebsite());
            cr.AddWebsite(AddGratkaWebsite());
            cr.AddSynonymsPropertiesToWebsite(AddGratkaManssionSynonyms(cr));
            cr.AddSynonymsPropertiesToWebsite(AddOLXManssionSynonyms(cr));

            cr.ShowWebpages();
            cr.StartLinkAnnouncementCrawler();
            cr.StartAnnouncementsCrawler();
            cr.Start();

            RequestsService rs = new();
            rs.PostUrl = "http://localhost:8080/announcement/many";
            rs.RegisterUrl = "http://localhost:8080/auth/register";
            rs.LoginUrl = "http://localhost:8080/auth/login";
            rs.AnnouncementsUrl = "http://localhost:8080/announcement/many";
            List<Announcement> announcements = cr.AnnouncementToSend();
            List<Announcement_manssion> announcement_Manssions = cr.AnnouncementManssionsToSend();

            rs.AddErrorLogService(new ErrorLogService("log.txt"));
            rs.StartErrorLogService();
            rs.Login("www@ww3w3.pl", "password");
            rs.Send(announcement_Manssions);
        }

        /// <summary>
        /// The function creates an array of synonyms for OLX
        /// </summary>
        /// <param name="webCrawler">WebCrawler class</param>
        /// <returns>Array of synonyms for OLX</returns>
        private static Announcement_manssion_synonyms[] AddOLXManssionSynonyms(WebCrawler webCrawler)
        {
            List<Announcement_manssion_synonyms> announcement_Manssion_Synonyms = new();
            Crawler_website cw = webCrawler.GetCrawlerAnnouncementId("https://www.olx.pl/d/nieruchomosci/");

            string[] Properties = new string[]
            {
                "RoomCount", "Level", "Furnished", "TypeOfBuild", "Area", 
                "YearOfConstruction", "Location", "Volume", "AdditionalArea", "PricePerM2",
                "LandArea", "Driveway", "State", "HeatingAndEnergy", "Media",
                "FenceOfThePlot", "ShapeOfThePlot", "Apperance", "NumberOfPositions", "BuildingMaterial",
                "Air_conditioning", "Balcony", "Basement", "Garage", "Garden",
                "Lift", "NonSmokingOnly", "SeparateKitchen", "Terrace", "TwoStoreys",
                "UtilityRoom", "AsphaltAccess", "Heating", "Parking", "Site",
                "TypeOfRoof", "Bungalow", "Recreational", "InvestmentStatus", "Internet",
                "CableTV", "Phone", "Preferences", "Market"
            };

            string[] values = new string[]
            {
                "Liczba pokoi:", "Poziom:", "Umeblowane:", "Rodzaj zabudowy:", "Powierzchnia:",
                "", "", "", "", "Cena za m²:",
                "Powierzchnia działki:", "", "Stan:", "", "",
                "", "", "", "", "",
                "", "", "", "", "",
                "", "", "", "", "",
                "", "", "", "", "",
                "", "", "", "", "",
                "", "", "", "Rynek:"
            };

            for (int i = 0; i < Properties.Length; i++)
            {
                if (values[i] is not "" && Properties[i] is not "")
                {
                    announcement_Manssion_Synonyms.Add(new Announcement_manssion_synonyms()
                    {
                        Value = values[i].Replace(" ", "").ToLower(),
                        Announcement_dictionary_mansion_properties = webCrawler.GetAnnouncementMansionSynonymPropertiesId(Properties[i]),
                        Crawler_Website = cw
                    });
                }

            }
            return announcement_Manssion_Synonyms.ToArray();
        }

        /// <summary>
        /// The function creates an array of synonyms for Gratka website
        /// </summary>
        /// <param name="webCrawler">WebCrawler class</param>
        /// <returns>Array of synonyms for gratka</returns>
        private static Announcement_manssion_synonyms[] AddGratkaManssionSynonyms(WebCrawler webCrawler)
        {
            List<Announcement_manssion_synonyms> announcement_Manssion_Synonyms = new();
            Crawler_website cw = webCrawler.GetCrawlerAnnouncementId("https://gratka.pl/nieruchomosci?sort=newest");

            string[] Properties = new string[]
            {
                "RoomCount", "Level", "Furnished", "TypeOfBuild", "Area",
                "YearOfConstruction", "Location", "Volume", "AdditionalArea", "PricePerM2",
                "LandArea", "Driveway", "State", "HeatingAndEnergy", "Media",
                "FenceOfThePlot", "ShapeOfThePlot", "Apperance", "NumberOfPositions", "BuildingMaterial",
                "Air_conditioning", "Balcony", "Basement", "Garage", "Garden",
                "Lift", "NonSmokingOnly", "SeparateKitchen", "Terrace", "TwoStoreys",
                "UtilityRoom", "AsphaltAccess", "Heating", "Parking", "Site",
                "TypeOfRoof", "Bungalow", "Recreational", "InvestmentStatus", "Internet",
                "CableTV", "Phone", "Preferences", "Market"
            };

            string[] values = new string[]
            {
                "Liczba pokoi", "Liczba pięter w budynku", "", "typbudynku", "", 
                "Rok budowy", "lokalizacja", "Głośność", "", "",
                "Powierzchnia działki w m2", "Droga dojazdowa", "Stan", "Ogrzewanie i energia", "Media",
                "Ogrodzenie działki", "", "", "Liczba stanowisk", "Materiał budynku",
                "", "", "", "", "",
                "", "", "", "", "",
                "", "", "", "Miejsce parkingowe", "",
                "", "", "", "", "",
                "", "", "", ""
            };
            
            for(int i = 0; i < Properties.Length; i++)
            {
                if (values[i] is not "" && Properties[i] is not "")
                {
                    announcement_Manssion_Synonyms.Add(new Announcement_manssion_synonyms()
                    {
                        Value = values[i].Replace(" ", "").ToLower(),
                        Announcement_dictionary_mansion_properties = webCrawler.GetAnnouncementMansionSynonymPropertiesId(Properties[i]),
                        Crawler_Website = cw
                    });
                }

            }
            return announcement_Manssion_Synonyms.ToArray();
        }

        /// <summary>
        /// Function to launch the crawler
        /// </summary>
        private static void CleanCrawlerSimulation()
        {
            // Configuring Timer
            _crawlerTimer.Interval = 2400;
            _crawlerTimer.AutoReset = true;

            // Declare Constants
            const string _dataBaseFile = "ProdCrawler.db";
            const string _registerUrl = "http://localhost:8080/auth/register";
            const string _loginUrl = "http://localhost:8080/auth/login";
            const string _announcementsUrl = "http://localhost:8080/announcement/many";
            const string _logFilePath = "ProdRequestLog.txt";
            const string _username = "www@ww3w3.pl";
            const string _password = "password";

            // Delete database file 
            File.Delete(_dataBaseFile);

            // Declare crawler
            _webCrawler = new(new DataContext("DataSource=" + _dataBaseFile));

            // Adding Gratka site crawling
            //_webCrawler.AddWebsite(AddGratkaWebsite());
            _webCrawler.AddWebsite(AddOLXWebsite());

            _webCrawler.AddSynonymsPropertiesToWebsite(AddOLXManssionSynonyms(_webCrawler));
            //_webCrawler.AddSynonymsPropertiesToWebsite(AddGratkaManssionSynonyms(_webCrawler));

            // Declare and run request service
            _requestsService = new()
            {
                RegisterUrl = _registerUrl,
                LoginUrl = _loginUrl,
                PostUrl = _announcementsUrl
            };
            _requestsService.AddErrorLogService(new ErrorLogService(_logFilePath));
            _requestsService.StartErrorLogService();
            _requestsService.Login(_username,_password);

            // Running crawler in background
            _crawlerTimer.Elapsed += OnTimedEvent;
            _crawlerTimer.Enabled = true;
            Console.ReadLine();
        }

        /// <summary>
        /// A cyclical function event that is used to download data from websites and send then to the API
        /// </summary>
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (_isRunning) return;

            FileStream fs = new FileStream("./console_out.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new(fs);

            Console.WriteLine(e.SignalTime);
            _isRunning = true;
            TextWriter backupOut = Console.Out;
            //Console.SetOut(TextWriter.Null);
            Console.SetOut(sw);
            _webCrawler.StartLinkAnnouncementCrawler();
            _webCrawler.StartAnnouncementsCrawler();
            List<Announcement_manssion> announcement_massions_to_send = _webCrawler.AnnouncementManssionsToSend();
            if (_requestsService.Send(announcement_massions_to_send))
                _webCrawler.SetSendStatus(announcement_massions_to_send);
            Console.SetOut(backupOut);
            sw.Close();
            fs.Close();
            _isRunning = false;
        }

        /// <summary>
        /// The function returns the entity of gratka Crawler_website
        /// </summary>
        /// <returns>Crawler_website</returns>
        private static Crawler_website AddGratkaWebsite()
        {
            Crawler_announcement ca = new()
            {
                //class == sticker__title
                Title_node_name = "h1",
                //class = priceInfo__value
                Price_node_name = "span",
                Image_node_name = "img",
                Image_class_name = "gallery__image",
                Image_attribute_value = "src",
                Description_class_name = "description__rolled",
                Description_node_name = "div",
                Crawler_Website_Link_Contains = new List<Crawler_announcements_link_contains>()
                { 
                    new Crawler_announcements_link_contains() { Value = "gratka"}
                }
            };

            return new Crawler_website()
            {
                Crawler_Announcement = ca,
                Website = "https://gratka.pl/nieruchomosci?sort=newest",
                // Ta nazwa wprowadza w błąd
                Regex = "article",
                Prelink = String.Empty,
                MaxPages = 2,
                Pagequery = "&page=",
                Link_attribute_value = "data-href"
            };
        }

        /// <summary>
        /// The function returns the entity of OLX Cralwer_website
        /// </summary>
        /// <returns>Crawler_website</returns>
        private static Crawler_website AddOLXWebsite()
        {
            Crawler_announcement ca = new()
            {
                Image_node_name = "img",
                Image_attribute_value = "data-src",
                Image_class_name = "css-1bmvjcs",
                Description_class_name = "css-g5mtbi-Text",
                Description_node_name = "div",
                Price_node_name = "h3",
                Title_node_name = "h1",
                Crawler_Website_Link_Contains = new List<Crawler_announcements_link_contains>()
                { 
                    new Crawler_announcements_link_contains(){ Value = "oferta"},
                    new Crawler_announcements_link_contains{ Value = "http", IsContains = false}
                }
            };

            return new Crawler_website()
            {
                Crawler_Announcement = ca,
                Website = "https://www.olx.pl/d/nieruchomosci/",
                Regex = "a",
                Prelink = "http://www.olx.pl",
                MaxPages = 2,
                Pagequery = "?page=",
                Link_attribute_value = "href"
            };
        }
    }
}
