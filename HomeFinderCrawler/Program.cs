using Crawler;
using DatabaseManager;
using DatabaseManager.Entities;
using RequestsServices;

namespace HomeFinderCrawler
{
    //TODO: Aplikacja nie powinna wyrzucać wyjątków, ale powinna logować wszystkie błędy do jakieś pliku, bądź może nawet tabeli skoro i tak już mamy stworzoną bazę danych
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
            {
                throw new ArgumentNullException(nameof(args));
            } 

            WebCrawler cr = new(new DataContext("DataSource=craw.db"));

            //Adding to database search template
            //cr.AddWebsite(AddOLXCrawler());
            cr.AddWebsite(AddGratkaWebsite());
            
            // TODO: Do zrobienia są synonimy w sposób dynamiczny.
            // AddGratkaSynonymsProperties()

            //cr.AddWebsite("https://www.otodom.pl/pl/oferty/sprzedaz/mieszkanie/cala-polska", "a", "http://www.otodom.pl", 2, new Crawler_announcement() { Image_node_name = "img" });

            cr.ShowWebpages();
            cr.StartLinkAnnouncementCrawler();
            cr.StartAnnouncementsCrawler();
            cr.Start();
            cr.Stop();

            // Sending requests to server
            /*           RequestsService rs = new();
                       rs.PostUrl = "https://webhook.site/4c97134c-cbf6-4ef0-a246-ded83f9ca632";
                       rs.RegisterUrl = "http://localhost:8080/auth/register";
                       rs.LoginUrl = "http://localhost:8080/auth/login";
                       rs.AnnouncementsUrl = "http://localhost:8080/announcement";
                       List<Announcement> announcements = cr.AnnouncementToSend();
                       rs.AddErrorLogService(new ErrorLogService("log.txt"));
                       rs.StartErrorLogService();
                       rs.Register("patryk@elo.pl", "password");
                       rs.Login("patryk@elo.pl", "password");
                       rs.AddAnnouncements(announcements);*/
            //rs.Send(announcements);

            // If sending was successfull, i must update database
            //cr.AnnoundementSend(announcements);

            //CleanCrawlerSimulation();
        }


        // Production Crawler Run Method
        private static void CleanCrawlerSimulation()
        {
            // Configuring Timer
            _crawlerTimer.Interval = 2000;
            _crawlerTimer.AutoReset = true;

            // Declare Constants
            const string _dataBaseFile = "ProdCrawler.db";
            const string _registerUrl = "http://localhost:8080/auth/register";
            const string _loginUrl = "http://localhost:8080/auth/login";
            const string _announcementsUrl = "http://localhost:8080/announcement";
            const string _logFilePath = "ProdRequestLog.txt";
            const string _username = "patryk@elo.pl";
            const string _password = "password";

            // Delete database file 
            File.Delete(_dataBaseFile);

            // Declare crawler
            _webCrawler = new(new DataContext("DataSource=" + _dataBaseFile));

            // Adding Gratka site crawling
            _webCrawler.AddWebsite(AddGratkaWebsite());

            // Declare and run request service
            _requestsService = new();
            _requestsService.RegisterUrl = _registerUrl;
            _requestsService.LoginUrl = _loginUrl;
            _requestsService.AnnouncementsUrl = _announcementsUrl;
            _requestsService.AddErrorLogService(new ErrorLogService(_logFilePath));
            _requestsService.StartErrorLogService();
            _requestsService.Login(_username,_password);

            // Running crawler in background
            _crawlerTimer.Elapsed += OnTimedEvent;
            _crawlerTimer.Enabled = true;
            Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if(_isRunning)
            {
                Console.WriteLine("Crawler is running. Skip");
                return;
            }

            Console.WriteLine(e.SignalTime);
            _isRunning = true;
            TextWriter backupOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            _webCrawler.StartLinkAnnouncementCrawler();
            _webCrawler.StartAnnouncementsCrawler();
            _requestsService.AddAnnouncements(_webCrawler.AnnouncementToSend());
            Console.SetOut(backupOut);
            _isRunning = false;
        }

        private static void AddGratkaSynonymsProperties(Crawler_announcement ca, Announcement_dictionary_mansion_properties announcement_Dictionary_Mansion_Properties)
        {
            // TODO: 

            Announcement_manssion_synonyms ams = new()
            {
                Crawler_Announcement = ca,
                Announcement_dictionary_mansion_properties = announcement_Dictionary_Mansion_Properties,
                Value = "lokalizacja"
            };


        }

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

        private static Crawler_website AddOLXCrawler()
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
