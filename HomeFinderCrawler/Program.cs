using Crawler;
using DatabaseManager;

namespace HomeFinderCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //WebCrawler cr = new(new DataContext("DataSource=file::memory:?cache=shared"));
            WebCrawler cr = new(new DataContext("DataSource=craw.db"));

            //Adding to database search template
            cr.AddWebsite("https://www.olx.pl/d/nieruchomosci/", "a");
            cr.ShowWebpages();
            cr.StartLinkAnnouncementCrawler();
            cr.StartAnnouncementsCrawler();
            //cr.Start();
            cr.Stop();
        }
    }
}
