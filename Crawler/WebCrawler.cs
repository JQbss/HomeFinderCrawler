using DatabaseManager;
using DatabaseManager.Entities;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Crawler
{
    public class WebCrawler
    {
        //Stron nie będzie dużo, może lepiej wczytywać je do tabicy sobie tutaj

        readonly DatabaseService _databaseService;

        public bool IsWorking { get; set; } = false;
        private bool _isRunning = false;

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
                    Console.WriteLine(crawler.Website + crawler.Pagequery + page);
                    HtmlDocument doc = web.Load($"{crawler.Website}{crawler.Pagequery}{page}");
                    HtmlNode[] nodes = doc.DocumentNode.SelectNodes($"//{crawler.Regex}").ToArray();

                    foreach (HtmlNode item in nodes)
                    {
                        string link = item.GetAttributeValue(crawler.Link_attribute_value, "");
                        Console.WriteLine(link);
                        bool control = true;
                        if (crawler.Crawler_Announcement.Crawler_Website_Link_Contains is not null)
                        {
                            foreach (var link_contains in crawler.Crawler_Announcement.Crawler_Website_Link_Contains)
                            {
                                if (link_contains.IsContains)
                                {
                                    if (!link.Contains(link_contains.Value))
                                    {
                                        control = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (link.Contains(link_contains.Value))
                                    {
                                        control = false;
                                        break;
                                    }
                                }
                            }
                        }

                        // Jeśli link pasuje do podango schematu to kontunuujmy badanie
                        if (!control) continue;


                        //TODO: Sprwadzanie czy ogłoszenie jest już w bazie danych
                        Announcement announcement1 = _databaseService.GetAnnouncementByLink(crawler.Prelink + link);
                        if (announcement1 != null)
                        {
                            Console.WriteLine("ogłoszenie jest aktualne");
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
                            announcements.Add(new() { Link = crawler.Prelink + link, Processed = false, Crawler_Website = crawler, Announcement_type = "manssion" });
                        }
                    }
                    page++;
                }
            }

            //Dodawanie do bazy danych
            foreach(Announcement announcement in announcements.Distinct())
                _databaseService.AddAnnouncement(announcement);

            _databaseService.SaveChanges();
        }
        
        //Crawlowanie nieprzetworzonych ogłoszeń
        public void StartAnnouncementsCrawler()
        {
            // Pobieranie nieprzetworzonych danych
            List<Announcement> announcements = _databaseService.GetAnnouncements().Where(x => x.Processed == false).ToList();
            Console.WriteLine(announcements.Count);

            foreach (Announcement announcement in announcements)
                StartAnnouncementCrawler(announcement.Link);

            _databaseService.SaveChanges();
        }

        private static void DownloadImagesUrl(Announcement announcement, HtmlDocument doc)
        {
            List<Image> images = new();
            foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Image_node_name}")
                .Where(x => x.HasClass(announcement.Crawler_Website.Crawler_Announcement.Image_class_name)))
            {
                string img = item.GetAttributeValue(announcement.Crawler_Website.Crawler_Announcement.Image_attribute_value, "");
                Console.WriteLine(img);
                if (!string.IsNullOrEmpty(img))
                    images.Add(new Image() { Url = img });
            }
            announcement.Images = images;
        }
        private static void DownloadTitle(Announcement announcement, HtmlDocument doc)
        {
            if (doc.DocumentNode.Descendants(announcement.Crawler_Website.Crawler_Announcement.Title_node_name).Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Title_node_name}"))
                    announcement.Title = item.InnerText.Trim();
            }
        }
        private static void DownloadPrice(Announcement announcement, HtmlDocument doc)
        {
            if (doc.DocumentNode.Descendants(announcement.Crawler_Website.Crawler_Announcement.Price_node_name).Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Price_node_name}"))
                {
                    //To jest dobrze i tak ma być
                    string tmp = Regex.Match(item.InnerText.Replace(" ", ""), @"\d+").Value;
                    if (!string.IsNullOrEmpty(tmp))
                        announcement.Price = Convert.ToInt32(tmp);
                }
            }
        }
        private void DownloadManssionProperties(Announcement announcement, HtmlDocument doc)
        {
            if (announcement is null || doc is null) return;
            // Sprawdzenie czy to ogłoszenie dotyczy nieruchomości
            // TODO: Trzeba zrobić to porządnie na tabelach, a nie robić hardcode
            if (announcement.Announcement_type is not null && announcement.Announcement_type.Equals("manssion"))
            {
                Console.WriteLine("OGŁOSZENIE JAKIEŚ NIERUCHOMOŚCI");

                //Sprawdzanie czy jest jakiś rekord w Announcement_manssion dla tej posesji
                var announcement_Manssion = _databaseService.GetAnnouncementManssionByAnnouncemenetId(announcement.Id);

                if (announcement_Manssion is null)
                {
                    announcement_Manssion = new Announcement_manssion();
                    announcement_Manssion.Announcement = _databaseService.GetAnnouncementById(announcement.Id);
                    DownloadManssionLevel(announcement, doc, announcement_Manssion);
                    //Pobieranie listy z parametrami

                    _databaseService.AddAnnouncementManssion(announcement_Manssion);
                }
            }
        }

        private void DownloadManssionLevel(Announcement announcement, HtmlDocument doc, Announcement_manssion announcement_Manssion)
        {
            Console.WriteLine("POBIERANIE LEVELU REZYDENCJI");

            //TODO: To można zrobić zupełnie uniwersalnie dla portali i szukać tylko w tej tablicy odpowiednich wzorców
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//ul").Where(x => x.HasClass("parameters__singleParameters")).ToArray();

            //TODO: Trzeba dodać do bazy danych odpowiednią tabelę, która ogarnie te synonimy i będzie wrzucać do odopwiednich danych -- na razie zasymuluję to tablicami
            foreach (var node in nodes)
            {
                //Teraz trzeba zrobić pętle po li
                foreach (var nod in node.SelectNodes("//li"))
                {

                    // każdy węzeł to inna właściwość
                    string[] locations_synonims = new string[1];
                    locations_synonims[0] = "lokalizacja";

                    if (announcement_Manssion.Localization is null)
                        announcement_Manssion.Localization = SearchFirstSynonymValue(nod, locations_synonims);

                    string[] level_synonyms = new string[1];
                    level_synonyms[0] = "Liczba pięter w budynku".Replace(" ","").ToLower();

                    if(announcement_Manssion.Level is null)
                        announcement_Manssion.Level = SearchFirstSynonymValue(nod, level_synonyms);

                    string[] area_synonyms = new string[1];
                    area_synonyms[0] = "powierzchniawm2";

                    //TODO: Trzeba zrobić castowanie danych
                   /* if (announcement_Manssion.Area is null)
                        announcement_Manssion.Area = SearchFirstSynonymValue(nod, area_synonyms);*/

                    string[] type_synonyms = new string[1];
                    type_synonyms[0] = "typbudynku";
                    if (announcement_Manssion.Type_of_building is null)
                        announcement_Manssion.Type_of_building = SearchFirstSynonymValue(nod, type_synonyms);

                    // TODO: Dane, które można pobrać
                    // forma własności

                    // Rok budowy
                    string[] year_synonyms = new string[1];
                    year_synonyms[0] = "Rok budowy".Replace(" ", "").ToLower();

                  //  if (announcement_Manssion.Year_od_construction is null)
                  //      announcement_Manssion.Year_od_construction = int.Parse(SearchFirstSynonymValue(nod, year_synonyms));

                    // Liczba pokoi
                    string[] room_synonyms = new string[1];
                    room_synonyms[0] = "Liczba pokoi".Replace(" ", "").ToLower();

                    if (announcement_Manssion.Room_count is null)
                        announcement_Manssion.Room_count = SearchFirstSynonymValue(nod, room_synonyms);

                    // Typ zabudowy
                    string[] type_of_building = new string[1];
                    room_synonyms[0] = "Liczba pokoi".Replace(" ", "").ToLower();

                }
            }
        }
        
        private string SearchFirstSynonymValue(HtmlNode node, string[] synonyms)
        {
            foreach (string synonym in synonyms)
            {
                string text = node.InnerText.Replace(" ", "").ToLower();
                if (text.Contains(synonym))
                {
                    text = text.Replace(synonym, "").Trim();
                    Console.WriteLine(text);
                    return text;
                }
            }
            return null;
        }


        private void StartAnnouncementCrawler(string url)
        {
            //TODO: Może lepiej sprawdzać po jakimś id bądź nazwie klasy to będzie wiele czytelniej.
            //TODO: Rozdzielić te funkcję na mniejsze funkcje.
            //Jeśli nie jest aktualne to trzeba usunąć

            // Przetwarzanie informacji o ogłoszeniu
            HtmlWeb web = new();
            Console.WriteLine(url);
            HtmlDocument doc = web.Load(url);
            HtmlNode[] nodes;
            Announcement announcement = _databaseService.GetAnnouncementByLink(url);

            Console.WriteLine(announcement.Link);
            //Pobieranie zdjęć ogłoszenia
            DownloadImagesUrl(announcement, doc);
            DownloadTitle(announcement, doc);
            DownloadPrice(announcement, doc);
            DownloadManssionProperties(announcement, doc);

            //Pobieranie opisu ogłoszenia
            //TODO: Zabezpieczyć, że nie zawsze może być podana klasa i takie tam.
            nodes = doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Description_node_name}")
                .Where(x => x.HasClass(announcement.Crawler_Website.Crawler_Announcement.Description_class_name)).ToArray();

            //Dlaczego tutaj są takie dziwne pętle
            //Tego nie powinno tutaj być
            foreach( HtmlNode item in nodes)
            {
                Console.WriteLine(item.InnerText);
                announcement.Description = item.InnerText.Trim();
            }

            // Updating announcement
            
            // TODO: Tymczasowo, żeby przetwarzać kilkra razy ogłoszenia
            // announcement.Processed = true;
            _databaseService.SaveChanges();
        }

        private bool WebsiteExists(string url)
        {
            foreach(var cw in _databaseService.GetCrawlerWebsites())
            {
                if (cw.Website.Equals(url))
                    return true;
            }
            return false;
        }

        public void AddSynonymsPropertiesToWebsite(Announcement_manssion_synonyms[] announcement_Manssion_Synonyms)
        {
            // TODO: Or jeśli synonim już istnieje
            if(announcement_Manssion_Synonyms is null) return;

            _databaseService.AddSynonymPropertiesWebsite(announcement_Manssion_Synonyms[0]);
        }

        public void AddWebsite(Crawler_website website)
        {
            if (website is null || WebsiteExists(website.Website)) return;

            _databaseService.AddCrawlerWebsite(website);
            _databaseService.SaveChanges();
        }

        public void AddWebsite(string url, string regex, string prelink, int maxPage, Crawler_announcement crawler_Announcement)
        {
            if (url is null || WebsiteExists(url)) return;

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