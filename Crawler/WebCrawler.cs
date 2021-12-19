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
            IsWorking = true;
            _isRunning = true;
        }

        public List<Announcement> AnnouncementToSend()
        {
            return _databaseService.GetAnnouncementsBySentAndProcessed(false, true);
        }

        public void AnnoundementSend(List<Announcement> announcements)
        {
            for(int i = 0; i < announcements.Count; i++)
                announcements[i].Sent = true;
            _databaseService.SaveChanges();
        }

        public void StartLinkAnnouncementCrawler()
        {
            //Do linku wystarczy dodawać ?page=2
            // ?page=3

            
            HtmlWeb web = new();
            //Czytanie linków z ogłoszeniami i dodawanie ich do bazy danych.
            //Doawaj dopóki nie znajdzie ogłoszenia, które jest już w bazie danych

            List<Crawler_website> crawler_Websites = _databaseService.GetCrawlerWebsites();
            List<Announcement> announcements = new();
            foreach (Crawler_website crawler in crawler_Websites)
            {
                int page = 1;
                while (page < crawler.MaxPages)
                {
                    HtmlDocument doc = web.Load($"{crawler.Website}{crawler.Pagequery}{page}");
                    HtmlNode[] nodes = doc.DocumentNode.SelectNodes($"//{crawler.Regex}").ToArray();

                    foreach (HtmlNode item in nodes)
                    {
                        string link = item.GetAttributeValue("href", "");

                        //TODO: To database
                        if (link.Contains("oferta") && !link.Contains("http"))
                        {

                            //TODO: Sprwadzanie czy ogłoszenie jest już w bazie danych
                            Announcement announcement1 = _databaseService.GetAnnouncementByLink(crawler.Prelink + link);
                            if (announcement1 != null)
                            {
                                Console.WriteLine("ogłoszenia są aktualne");
                                /*foreach (Announcement announcement in announcements.Distinct())
                                {
                                    _databaseService.AddAnnouncement(announcement);
                                }
                                _databaseService.SaveChanges();
                                return;*/
                            }
                            else
                            {
                                //Adding data to database
                                Console.WriteLine(crawler.Prelink + link);

                                announcements.Add(new() { Link = crawler.Prelink + link, Processed = false, Crawler_Website = crawler });
                                //_databaseService.AddAnnouncement(announcement);
                            }
                        }
                        else
                        {
                            //Console.WriteLine(link);
                        }
                    }
                    page++;
                }
            }

            //Dodawanie do bazy danych

            foreach(Announcement announcement in announcements.Distinct())
            {
                _databaseService.AddAnnouncement(announcement);
            }

            _databaseService.SaveChanges();
        }
        
        //Crawlowanie nieprzetworzonych ogłoszeń
        public void StartAnnouncementsCrawler()
        {
            // Pobieranie nieprzetworzonych danych

            List<Announcement> announcements = _databaseService.GetAnnouncements().Where(x => x.Processed == false).ToList();
            Console.WriteLine(announcements.Count);
            foreach (Announcement announcement in announcements)
            {
                if(announcement.Link.Contains("olx"))
                {
                    StartAnnouncementCrawler(announcement.Link);
                }
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
            HtmlNode[] nodes;
            Announcement announcement = _databaseService.GetAnnouncementByLink(url);

            //Pobieranie zdjęć ogłoszenia
            List<Image> images = new();
            foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Image_node_name}"))
            {
                string img = item.GetAttributeValue("data-src", "");
                if (!string.IsNullOrEmpty(img))
                {
                    images.Add(new Image() { Url = img });
                    //Console.WriteLine(img);
                }
            }

            //Pobieranie tytułu ogłoszenia
            if (doc.DocumentNode.Descendants("h1").Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//h1"))
                {
                    announcement.Title = item.InnerText;
                    //Console.WriteLine(item.InnerText);
                }
            }


            // Pobieranie ceny
            if (doc.DocumentNode.Descendants("h3").Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//h3"))
                {
                    string tmp = Regex.Match(item.InnerText.Replace(" ", ""), @"\d+").Value;
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        announcement.Price = Convert.ToInt32(tmp);
                        //Console.WriteLine(tmp);
                    }
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
            _databaseService.SaveChanges();
        }

        public void AddWebsite(string url, string regex, string prelink, int maxPage, Crawler_announcement crawler_Announcement)
        {
            //Check if exists
            //TODO: Checking Exists
            List<Crawler_website> cw = _databaseService.GetCrawlerWebsites();

            bool exists = false;
            for(int i=0;i<cw.Count;i++)
            {
                if(cw[i].Website == url)
                {
                    exists = true;
                    break;
                }
            }

            if(!exists)
            {
                Crawler_website ncw = new()
                {
                    Regex = regex,
                    Website = url,
                    Prelink = prelink,
                    MaxPages = maxPage,
                    Crawler_Announcement = crawler_Announcement
                };

                _databaseService.AddCrawlerWebsite(ncw);
                _databaseService.SaveChanges();
            }
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