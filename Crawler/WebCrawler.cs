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
                        Announcement? announcement1 = _databaseService.GetAnnouncementByLink(crawler.Prelink + link);
                        if (announcement1 is not null)
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

        // Pobieranie danych o ogłoszeniu
        private void DownloadManssionLevel(Announcement announcement, HtmlDocument doc, Announcement_manssion announcement_Manssion)
        {
            int websiteID = _databaseService.GetAnnonucementCrawlerWebsiteId(announcement_Manssion.Announcement.Id);
            // TODO: To można zrobić zupełnie uniwersalnie dla portali i szukać tylko w tej tablicy odpowiednich wzorców
            // Wydaje mi się, że te portale z założenia 
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//ul").Where(x => x.HasClass("parameters__singleParameters")).ToArray();

            //TODO: Trzeba dodać do bazy danych odpowiednią tabelę, która ogarnie te synonimy i będzie wrzucać do odopwiednich danych -- na razie zasymuluję to tablicami
            foreach (var node in nodes)
            {
                //Teraz trzeba zrobić pętle po li
                foreach (var nod in node.SelectNodes("//li"))
                {
                    // Lokalizacja ogłoszenia
                    string[] locations_synonims = _databaseService.GetAnnouncementsManssionSynonyms("Location", websiteID);
                    if (announcement_Manssion.Localization is null)
                        announcement_Manssion.Localization = SearchFirstSynonymValue(nod, locations_synonims);

                    // Liczba pięter w budynku
                    string[] level_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Level", websiteID);
                    if(announcement_Manssion.Level is null)
                        announcement_Manssion.Level = SearchFirstSynonymValue(nod, level_synonyms);

                    // Powierzchnnia w metrach kwadratowych
                    string[] area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Area", websiteID);
                    //TODO: Zły format danych w bazie - ogarnia tylko liczby
                    if (announcement_Manssion.Area is null)
                    {
                        announcement_Manssion.Area = SearchFirstSynonymValue(nod, area_synonyms) is null ? null: double.Parse(SearchFirstSynonymValue(nod, area_synonyms).Replace("m2",""));
                    }

                    // Typ budynku
                    string[] type_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("TypeOfBuild", websiteID);
                    if (announcement_Manssion.Type_of_building is null)
                        announcement_Manssion.Type_of_building = SearchFirstSynonymValue(nod, type_synonyms);

                    // Rok budowy
                    string[] year_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("YearOfConstruction", websiteID);
                    if (announcement_Manssion.Year_od_construction is null)
                        announcement_Manssion.Year_od_construction = SearchFirstSynonymValue(nod, year_synonyms) is null ? null: int.Parse(SearchFirstSynonymValue(nod, year_synonyms));

                    // Liczba pokoi
                    string[] room_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("RoomCount", websiteID);
                    if (announcement_Manssion.Room_count is null)
                        announcement_Manssion.Room_count = SearchFirstSynonymValue(nod, room_synonyms);

                    // Typ zabudowy
                    string[] type_of_building = _databaseService.GetAnnouncementsManssionSynonyms("TypeOfBuild", websiteID);
                    if (announcement_Manssion.Type_of_building is null)
                        announcement_Manssion.Type_of_building = SearchFirstSynonymValue(nod, type_of_building);

                    // Cena wynajmu
                    string[] Rent_price_synonyms = new string[1];

                    string[] year_of_construction_synonyms = new string[1];

                    // Głośność mieszkania
                    string[] volume_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Volume", websiteID);
                    if(announcement_Manssion.Volume is null)
                        announcement_Manssion.Volume = SearchFirstSynonymValue(nod, volume_synonyms);

                    // Powierzchnia dodatkowa z gratki = Tutaj może być ich przecież wiele
                    string[] additional_area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("AdditionalArea", websiteID);
                    if (announcement_Manssion.Additional_area is null)
                        announcement_Manssion.Additional_area = SearchFirstSynonymValue(nod, additional_area_synonyms);

                    string[] price_per_m2_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("PricePerM2", websiteID);
                    if (announcement_Manssion.Price_per_m2 is null)
                        announcement_Manssion.Additional_area = SearchFirstSynonymValue(nod, price_per_m2_synonyms);

                    // Powierzchnia działki w metrach kwaratowych
                    string[] land_area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("LandArea", websiteID);
                    if (announcement_Manssion.Land_area is null)
                        announcement_Manssion.Land_area = SearchFirstSynonymValue(nod, land_area_synonyms);

                    // Droga dojazdowa z gratki
                    string[] driveway_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Driveway", websiteID);
                    if (announcement_Manssion.Driveway is null)
                        announcement_Manssion.Driveway = SearchFirstSynonymValue(nod, driveway_synonyms);

                    // Stan nieruchomości z gratki
                    string[] state_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("State", websiteID);
                    if (announcement_Manssion.State is null)
                        announcement_Manssion.State = SearchFirstSynonymValue(nod, state_synonyms);

                    // Ogrzewanie i energia z gratki
                    string[] heating_and_energy_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("HeatingAndEnergy", websiteID);
                    if (announcement_Manssion.Heating_and_energy is null)
                        announcement_Manssion.Heating_and_energy = SearchFirstSynonymValue(nod, heating_and_energy_synonyms);

                    // Media z gratki
                    string[] media_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Media", websiteID);
                    if (announcement_Manssion.Media is null)
                        announcement_Manssion.Media = SearchFirstSynonymValue(nod, media_synonyms);

                    // Ogrodzenie działki z gratki
                    string[] fence_of_the_plot_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("FenceOfThePlot", websiteID);
                    if (announcement_Manssion.Fence_of_the_plot is null)
                        announcement_Manssion.Fence_of_the_plot = SearchFirstSynonymValue(nod, fence_of_the_plot_synonyms);

                    string[] shape_of_the_plot_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("ShapeOfThePlot", websiteID);
                    if (announcement_Manssion.Shape_of_the_plot is null)
                        announcement_Manssion.Shape_of_the_plot = SearchFirstSynonymValue(nod, shape_of_the_plot_synonyms);

                    string[] apperance_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Apperance", websiteID);
                    if (announcement_Manssion.Apperance is null)
                        announcement_Manssion.Apperance = SearchFirstSynonymValue(nod, apperance_synonyms);

                    // Liczba stanowisk w garażu
                    string[] number_of_position_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("NumberOfPositions", websiteID);
                    if (announcement_Manssion.Number_of_positions is null)
                        announcement_Manssion.Number_of_positions = SearchFirstSynonymValue(nod, number_of_position_synonyms);

                    // Materiał budynku z Gratki
                    string[] building_material_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("BuildingMaterial", websiteID);
                    if(announcement_Manssion.Building_material is null)
                        announcement_Manssion.Building_material = SearchFirstSynonymValue(nod, building_material_synonyms);

                    string[] air_condition_synonyms = new string[1];

                    string[] balcony_synonyms = new string[1];

                    string[] basement_synonyms = new string[1];

                    string[] garage_synonyms = new string[1];

                    string[] garden_synonyms = new string[1];

                    string[] lift_synonyms = new string[1];

                    string[] non_smoking_only_synonyms = new string[1];

                    string[] separate_kitchen_synonyms = new string[1];

                    string[] terrace_synonyms = new string[1];

                    string[] two_storeys_synonyms = new string[1];

                    string[] asphalt_access_synonyms = new string[1];

                    string[] heating_synonyms = new string[1];

                    // TODO: Miejsce parkingowe z gratki
                    // TODO: Do przetestowania - nie wiem czy boolean to jest tutaj dobry pomysł, albo niech będzie boolean który możę mieć null
                    string[] parking_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Parking", websiteID);
                    if (!announcement_Manssion.Parking)
                        announcement_Manssion.Parking = SearchFirstSynonymValue(nod, parking_synonyms) is null ? false : true;


                    string[] site_synonyms = new string[1];

                    string[] type_of_roof_synonyms = new string[1];

                    string[] bungalow_synonyms = new string[1];

                    string[] recreational_synonyms = new string[1];

                    string[] investment_status_synonyms = new string[1];

                    string[] internet_synonyms = new string[1];

                    string[] cable_tv_synonyms = new string[1];

                    string[] phone_synonyms = new string[1];

                    string[] preferences_synonyms = new string[1];

                    string[] market_synonyms = new string[1];

                    // TODO: Jest jeszcze na gratce właściwość OKNA
                    // TODO: Jest jeszcze właściwość: Czy mieszkanie ma łazienkę.
                    // TODO: Jest jeszcze liczba miejsc parkingowych
                    // TODO: User może zacznaczyć wiele obiektów w pobliżu.
                    // TODO: Czy jest prąd, czy jest gazm czy jest woda
                    // TODO: Kanalizacja
                    // TODO: Garaże mogą mieć jeszcze pole: konstrukcja

                }
            }
        }
        
        // TODO: Zrobić to porządnie, żeby nie ucinała tekstu i spacji i tym podobne
        private string SearchFirstSynonymValue(HtmlNode node, string[] synonyms)
        {
            foreach (string synonym in synonyms.Where(x => x is not null && synonyms.Length != 0))
            {
                string text = node.InnerText.Replace(" ", "").ToLower();
                if (text.Contains(synonym))
                {
                    text = text.Replace(synonym, "").Trim();
                    Console.WriteLine(text);
                    return text;
                }
            }

            // TODO: Tutaj można ogarnąć, żeby przerobić to na string.empty
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
            Announcement? announcement = _databaseService.GetAnnouncementByLink(url);

            if (announcement is null)
                return;

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

        /// <summary>
        /// Dodawanie synonimów do strony internetowej
        /// </summary>
        /// <param name="announcement_Manssion_Synonyms">Tablica synonimów</param>
        public void AddSynonymsPropertiesToWebsite(Announcement_manssion_synonyms[] announcement_Manssion_Synonyms)
        {
            // Sprawdzanie czy tablica nie jesst pusta
            if(announcement_Manssion_Synonyms is null || announcement_Manssion_Synonyms.Length == 0)
                return;

            foreach(var synonym in announcement_Manssion_Synonyms)
            {
                // Sprawdzanie czy synonim jest już w bazie danych
                bool exists = _databaseService.CheckSynonymExists(synonym.Crawler_Website.Id, synonym.Announcement_dictionary_mansion_properties.Id, synonym.Value);
                if (!exists)
                    _databaseService.AddSynonymPropertiesWebsite(synonym);
            }

            _databaseService.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property_name"></param>
        /// <returns></returns>
        public Announcement_dictionary_mansion_properties? GetAnnouncementMansionSynonymPropertiesId(string property_name)
        {
            return _databaseService.GetAnnonuncementDictionaryMansionPropertiesId(property_name);
        }

        public Crawler_website? GetCrawlerAnnouncementId(string webName)
        {
            return _databaseService.GetCrawlerAnnouncementId(webName);
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