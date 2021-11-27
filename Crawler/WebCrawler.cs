using DatabaseManager;
using DatabaseManager.Entities;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Crawler
{
    public class WebCrawler
    {
        //Stron nie będzie dużo, może lepiej wczytywać je do tabicy sobie tutaj

        readonly DatabaseService _databaseService;

        public bool IsWorking { get; set; } = false;
        private bool _isRunning { get; set; } = false;

        public WebCrawler(DataContext dataContext)
        {
            _databaseService = new(dataContext);
        }

        public void Start()
        {
            _databaseService.Test();
            IsWorking = true;
            _isRunning = true;
        }

        public void StartCrawling()
        {


        }


        public void StartLinkAnnouncementCrawler()
        {
            //Do linku wystarczy dodawać ?page=2
            // ?page=3

            int page = 1;
            HtmlWeb web = new();
            //Czytanie linków z ogłoszeniami i dodawanie ich do bazy danych.
            //Doawaj dopóki nie znajdzie ogłoszenia, które jest już w bazie danych

            List<Crawler_website> crawler_Websites = _databaseService.GetCrawlerWebsites();
            foreach (Crawler_website crawler in crawler_Websites)
            {
                while(page < 20)
                {
                    HtmlDocument doc = web.Load($"{crawler.Website}?page={page}");
                    HtmlNode[] nodes = doc.DocumentNode.SelectNodes($"//{crawler.Regex}").ToArray();

                    foreach (HtmlNode item in nodes)
                    {
                        string link = item.GetAttributeValue("href", "");

                        if (link.Contains("oferta") && !link.Contains("http"))
                        {

                            //TODO: Sprwadzanie czy ogłoszenie jest już w bazie danych
                            Announcement announcement1 = _databaseService.GetAnnouncementByLink("http://olx.pl" + link);
                            if (announcement1 != null)
                            {
                                Console.WriteLine("ogłoszenia są aktualne");
                                return;
                            }

                            //Adding data to database
                            Console.WriteLine("http://olx.pl" + link);

                            Announcement announcement = new() { link = "http://olx.pl" + link, Processed = false };
                            _databaseService.AddAnnouncement(announcement);
                        }
                        else
                        {
                            //Console.WriteLine(link);
                        }
                    }
                    page++;
                }
            }
            _databaseService.SaveChanges();
        }
        
        //Crawlowanie nieprzetworzonych ogłoszeń
        public void StartAnnouncementsCrawler()
        {
            // Pobieranie nieprzetworzonych danych
            List<Announcement> announcements = _databaseService.GetAnnouncements().Where(x => x.Processed == false).ToList();
            foreach (Announcement announcement in announcements)
            {
                StartAnnouncementCrawler(announcement.link);
            }
            _databaseService.SaveChanges();
        }

        private void StartAnnouncementCrawler(string url)
        {
            //TODO: Zanim zacznę przetwarzać muszę sprwadzać czy ogłozenie jest w ogóle jeszcze aktualne
            //Jeśli nie jest aktualne to trzeba usunąć

            // Przetwarzanie informacji o ogłoszeniu
            HtmlWeb web = new();
            Console.WriteLine(url);
            HtmlDocument doc = web.Load(url);
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//img").ToArray();

            //Pobieranie zdjęć ogłoszenia
            List<Image> images = new();
            foreach (HtmlNode item in nodes)
            {
                string img = item.GetAttributeValue("data-src", "");
                if (!string.IsNullOrEmpty(img))
                {
                    images.Add(new Image() { Url = img });
                    //Console.WriteLine(img);
                }
            }
            Announcement announcement = _databaseService.GetAnnouncementByLink(url);

            //Pobieranie tytułu ogłoszenia
            nodes = doc.DocumentNode.SelectNodes("//h1").ToArray();
            foreach(HtmlNode item in nodes)
            {
                announcement.Title = item.InnerText;
                //Console.WriteLine(item.InnerText);
            }

            // Pobieranie ceny
            nodes = doc.DocumentNode.SelectNodes("//h3").ToArray();
            foreach (HtmlNode item in nodes)
            {
                string tmp = Regex.Match(item.InnerText.Replace(" ", ""), @"\d+").Value;
                if(!string.IsNullOrEmpty(tmp))
                {
                    announcement.Price = Convert.ToInt32(tmp);
                    //Console.WriteLine(tmp);
                }
            }

            // Pobieranie danych z opisu ogłoszenia
            // div class="css-g5mtbi-Text"

            nodes = doc.DocumentNode.SelectNodes("//div").Where(x => x.HasClass("css-g5mtbi-Text")).ToArray();

            foreach( HtmlNode item in nodes)
            {
                Console.WriteLine(item.InnerText);
                announcement.Description = item.InnerText;
            }

            // Pobieranie ogłoszenia z bazy po linku i jego aktualizacja
            announcement.Images = images;
            announcement.Processed = true;
        }

        public void AddWebsite(string url, string regex)
        {
            Crawler_website cw = new()
            {
                Regex = regex,
                Website = url
            };

            _databaseService.AddCrawlerWebsite(cw);
            _databaseService.SaveChanges();
        }

        //TODO:
        public void RemoveWebsite(string url)
        {
            _databaseService.RemoveCrawlerWebsite(url);
        }

        //TEST METHOD - TO DELETE
        public void ShowWebpages()
        {
            List<Crawler_website> crawler_Websites = _databaseService.GetCrawlerWebsites();

            for(int i = 0; i < crawler_Websites.Count; i++)
            {
                Console.WriteLine(crawler_Websites[i].Id + " " + crawler_Websites[i].Website + " " + crawler_Websites[i].Regex);
            }
        }

        public void Stop()
        {
            IsWorking = false;
            _isRunning = false;
            Console.WriteLine("STOP");
        }
    }
}