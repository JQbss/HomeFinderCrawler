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
            cr.AddWebsite("https://www.olx.pl/d/nieruchomosci/", "a", "http://www.olx.pl", 2, new Crawler_announcement() { Image_node_name = "img" });
            cr.AddWebsite("https://www.otodom.pl/pl/oferty/sprzedaz/mieszkanie/cala-polska", "a", "http://www.otodom.pl", 2, new Crawler_announcement() { Image_node_name = "img" });
            cr.ShowWebpages();
            //cr.StartLinkAnnouncementCrawler();
            //cr.StartAnnouncementsCrawler();
            cr.Start();
            cr.Stop();

            // Sending requests to server 
            // TODO: Nie zwraca obrazów w ogłoszeniach...
            RequestsService rs = new();
            List<Announcement> announcements = cr.AnnouncementToSend();
            rs.Send(announcements);

            // If sending was successfull, i must update database
            //cr.AnnoundementSend(announcements);
        }
    }
}
