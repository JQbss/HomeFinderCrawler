using Crawler;
using DatabaseManager;
using DatabaseManager.Entities;
using RequestsServices;

namespace HomeFinderCrawler
{
    //TODO: Aplikacja nie powinna wyrzucać wyjątków, ale powinna logować wszystkie błędy do jakieś pliku, bądź może nawet tabeli skoro i tak już mamy stworzoną bazę danych
    public class Program
    {
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
            //cr.AddWebsite("https://www.otodom.pl/pl/oferty/sprzedaz/mieszkanie/cala-polska", "a", "http://www.otodom.pl", 2, new Crawler_announcement() { Image_node_name = "img" });
            

            cr.ShowWebpages();
            cr.StartLinkAnnouncementCrawler();
            cr.StartAnnouncementsCrawler();
            cr.Start();
            cr.Stop();

            // Sending requests to server
            RequestsService rs = new();
            rs.PostUrl = "https://webhook.site/4c97134c-cbf6-4ef0-a246-ded83f9ca632";
            List<Announcement> announcements = cr.AnnouncementToSend();
            rs.Login("email","password");
            rs.Send(announcements);

            // If sending was successfull, i must update database
            //cr.AnnoundementSend(announcements);
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
                Website = "https://gratka.pl/nieruchomosci",
                // Ta nazwa wprowadza w błąd
                Regex = "article",
                Prelink = String.Empty,
                MaxPages = 2,
                Pagequery = "?page=",
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
