using Crawler;
using DatabaseManager;
using DatabaseManager.Entities;
using RequestsServices;

namespace HomeFinderCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebCrawler cr = new(new DataContext("DataSource=craw.db"));

            //Adding to database search template
            //cr.AddWebsite(AddOLXCrawler());
            //cr.AddWebsite(AddGratkaWebsite());
            //cr.AddWebsite("https://www.otodom.pl/pl/oferty/sprzedaz/mieszkanie/cala-polska", "a", "http://www.otodom.pl", 2, new Crawler_announcement() { Image_node_name = "img" });
            

            cr.ShowWebpages();
            //cr.StartLinkAnnouncementCrawler();
            //cr.StartAnnouncementsCrawler();
            cr.Start();
            cr.Stop();

            // Sending requests to server
            RequestsService rs = new();
            rs.PostUrl = "https://webhook.site/6f2dba85-9c29-44b9-a274-f86e36295fc1";
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
                Description_node_name = "div"
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
                Title_node_name = "h1"
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
