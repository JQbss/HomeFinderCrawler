using DatabaseManager;
using DatabaseManager.Entities;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Crawler
{
    public class WebCrawler
    {
        readonly DatabaseService _databaseService;

        public bool IsWorking { get; set; } = false;
        private bool _isRunning = false;

        public WebCrawler(DataContext dataContext) => _databaseService = new(dataContext);

        /// <summary>
        /// Function to start the webCrawler
        /// </summary>
        public void Start()
        {
            IsWorking = true;
            _isRunning = true;
        }

        /// <summary>
        /// A function that retrieves the data to be sent
        /// </summary>
        /// <returns>Announcement_manssion entity list to be sent</returns>
        public List<Announcement_manssion> AnnouncementManssionsToSend() => _databaseService.GetAnnouncementManssionBySendAndProcessed(false, true);

        /// <summary>
        /// A function that retrieves the data to be sent
        /// </summary>
        /// <returns>Announcement entity list to be sent</returns>
        public List<Announcement> AnnouncementToSend() => _databaseService.GetAnnouncementsBySentAndProcessed(false, false);

        /// <summary>
        /// Function starts data crawling
        /// </summary>
        public void StartLinkAnnouncementCrawler()
        {
            HtmlWeb web = new();
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

                    int i = 0;
                    foreach (HtmlNode item in nodes)
                    {
                        string link = item.GetAttributeValue(crawler.Link_attribute_value, "");
                        bool control = true;
                        if (crawler.Crawler_Announcement.Crawler_Website_Link_Contains is not null)
                        {
                            foreach (var link_contains in crawler.Crawler_Announcement.Crawler_Website_Link_Contains)
                            {
                                if (link_contains.IsContains && !link.Contains(link_contains.Value))
                                {
                                    control = false;
                                    break;
                                }
                                else if (!link_contains.IsContains && link.Contains(link_contains.Value))
                                {
                                    control = false;
                                    break;
                                }
                            }
                        }

                        if (!control) continue;

                        Announcement? announcement1 = _databaseService.GetAnnouncementByLink(crawler.Prelink + link);
                        if (announcement1 is null)
                        {
                            string location = string.Empty;
                            Console.WriteLine(crawler.Prelink + link);
                            var nod = item.SelectNodes("//p").Where(x => x.HasClass("css-p6wsjo-Text")).ToArray()[i];
                            if (nod is not null)
                            {
                                location = Regex.Replace(nod.InnerText.Split(" - ")[0], "([a-z])([A-Z])", "$1 $2");
                                i++;

                            }
                            announcements.Add(new()
                            {
                                Link = crawler.Prelink + link,
                                Processed = false,
                                Crawler_Website = crawler,
                                Announcement_type = "manssion",
                                Localization = location
                            });

                        }
                    }
                    page++;
                }
            }

            //Dodawanie do bazy danych
            foreach(Announcement announcement in announcements.Distinct())
                _databaseService.AddAnnouncement(announcement);

            Console.WriteLine("zapisywanie");
            _databaseService.SaveChanges();
        }
        
        /// <summary>
        /// The function starts crawling unprocessed ads
        /// </summary>
        public void StartAnnouncementsCrawler()
        {
            List<Announcement> announcements = _databaseService.GetAnnouncements().Where(x => x.Processed == false).ToList();
            
            foreach (Announcement announcement in announcements)
            {
                StartAnnouncementCrawler(announcement.Link);
                announcement.Processed = true;
            }

            _databaseService.SaveChanges();
        }

        /// <summary>
        /// The function download images url
        /// </summary>
        /// <param name="announcement"> Announcemenet</param>
        /// <param name="doc"> HtmlDocument to reading</param>
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

        /// <summary>
        /// Function that gets the title of an advertisement
        /// </summary>
        /// <param name="announcement">Announcement</param>
        /// <param name="doc"> HtmlDocument to reading</param>
        private static void DownloadTitle(Announcement announcement, HtmlDocument doc)
        {
            if (doc.DocumentNode.Descendants(announcement.Crawler_Website.Crawler_Announcement.Title_node_name).Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Title_node_name}"))
                    announcement.Title = item.InnerText.Trim();
            }
        }

        /// <summary>
        /// Function that download the price of an avertisement
        /// </summary>
        /// <param name="announcement">Announcement</param>
        /// <param name="doc">HtmlDocument to reading</param>
        private static void DownloadPrice(Announcement announcement, HtmlDocument doc)
        {
            if (doc.DocumentNode.Descendants(announcement.Crawler_Website.Crawler_Announcement.Price_node_name).Any())
            {
                foreach (HtmlNode item in doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Price_node_name}"))
                {
                    string tmp = Regex.Match(item.InnerText.Replace(" ", ""), @"\d+").Value;
                    if (!string.IsNullOrEmpty(tmp))
                        announcement.Price = Convert.ToInt32(tmp);
                }
            }
        }

        /// <summary>
        /// Function responsible for retrieving avertisement details
        /// </summary>
        /// <param name="announcement">Announcement</param>
        /// <param name="doc">HtmlDocument to reading</param>
        private void DownloadManssionProperties(Announcement announcement, HtmlDocument doc)
        {
            if (announcement is null || doc is null) return;
            if (announcement.Announcement_type is not null && announcement.Announcement_type.Equals("manssion"))
            {
                var announcement_Manssion = _databaseService.GetAnnouncementManssionByAnnouncemenetId(announcement.Id);

                if (announcement_Manssion is not null) return;

                announcement_Manssion = new Announcement_manssion
                {
                    Announcement = _databaseService.GetAnnouncementById(announcement.Id)
                };
                DownloadManssionProperties(announcement, doc, announcement_Manssion);
                _databaseService.AddAnnouncementManssion(announcement_Manssion);
            }
        }

        /// <summary>
        /// Function responsible for retrieving avertisement details
        /// </summary>
        /// <param name="announcement">Announcement</param>
        /// <param name="doc">HtmlDocument to reading</param>
        /// <param name="announcement_Manssion"> Announcement manssion</param>
        private void DownloadManssionProperties(Announcement announcement, HtmlDocument doc, Announcement_manssion announcement_Manssion)
        {
            int websiteID = _databaseService.GetAnnonucementCrawlerWebsiteId(announcement_Manssion.Announcement.Id);
            HtmlNode[] nodes; 

            // O tym trzeba pamiętać
            if (announcement.Crawler_Website.Id == 2)
                    nodes = doc.DocumentNode.SelectNodes("//ul").Where(x => x.HasClass("parameters__singleParameters")).ToArray();
            // Ogłoszenia z OLX
            else
                nodes = doc.DocumentNode.SelectNodes("//ul").Where(x => x.HasClass("css-sfcl1s")).ToArray();

            foreach (var node in nodes)
            {
                foreach (var nod in node.SelectNodes("//li"))
                {
                    string[] locations_synonims = _databaseService.GetAnnouncementsManssionSynonyms("Location", websiteID);
                    if (announcement_Manssion.Localization is null)
                        announcement_Manssion.Localization = SearchFirstSynonymValue(nod, locations_synonims);

                    string[] level_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Level", websiteID);
                    if(announcement_Manssion.Level is null)
                        announcement_Manssion.Level = SearchFirstSynonymValue(nod, level_synonyms);

                    string[] area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Area", websiteID);
                    if (announcement_Manssion.Area is null)
                        announcement_Manssion.Area = SearchFirstSynonymValue(nod, area_synonyms) is null ? null: double.Parse(Regex.Replace(SearchFirstSynonymValue(nod, area_synonyms), "[^0-9.]", ""));

                    string[] type_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("TypeOfBuild", websiteID);
                    if (announcement_Manssion.Type_of_building is null)
                        announcement_Manssion.Type_of_building = SearchFirstSynonymValue(nod, type_synonyms);

                    string[] year_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("YearOfConstruction", websiteID);
                    if (announcement_Manssion.Year_od_construction is null)
                        announcement_Manssion.Year_od_construction = SearchFirstSynonymValue(nod, year_synonyms) is null ? null: int.Parse(SearchFirstSynonymValue(nod, year_synonyms));

                    string[] room_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("RoomCount", websiteID);
                    if (announcement_Manssion.Room_count is null)
                        announcement_Manssion.Room_count = SearchFirstSynonymValue(nod, room_synonyms);

                    string[] type_of_building = _databaseService.GetAnnouncementsManssionSynonyms("TypeOfBuild", websiteID);
                    if (announcement_Manssion.Type_of_building is null)
                        announcement_Manssion.Type_of_building = SearchFirstSynonymValue(nod, type_of_building);

                    string[] Rent_price_synonyms = new string[1];

                    string[] volume_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Volume", websiteID);
                    if(announcement_Manssion.Volume is null)
                        announcement_Manssion.Volume = SearchFirstSynonymValue(nod, volume_synonyms);

                    string[] additional_area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("AdditionalArea", websiteID);
                    if (announcement_Manssion.Additional_area is null)
                        announcement_Manssion.Additional_area = SearchFirstSynonymValue(nod, additional_area_synonyms);

                    string[] price_per_m2_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("PricePerM2", websiteID);
                    if (announcement_Manssion.Price_per_m2 is null)
                        announcement_Manssion.Additional_area = SearchFirstSynonymValue(nod, price_per_m2_synonyms);

                    string[] land_area_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("LandArea", websiteID);
                    if (announcement_Manssion.Land_area is null)
                        announcement_Manssion.Land_area = SearchFirstSynonymValue(nod, land_area_synonyms);

                    string[] driveway_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Driveway", websiteID);
                    if (announcement_Manssion.Driveway is null)
                        announcement_Manssion.Driveway = SearchFirstSynonymValue(nod, driveway_synonyms);

                    string[] state_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("State", websiteID);
                    if (announcement_Manssion.State is null)
                        announcement_Manssion.State = SearchFirstSynonymValue(nod, state_synonyms);

                    string[] heating_and_energy_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("HeatingAndEnergy", websiteID);
                    if (announcement_Manssion.Heating_and_energy is null)
                        announcement_Manssion.Heating_and_energy = SearchFirstSynonymValue(nod, heating_and_energy_synonyms);

                    string[] media_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Media", websiteID);
                    if (announcement_Manssion.Media is null)
                        announcement_Manssion.Media = SearchFirstSynonymValue(nod, media_synonyms);

                    string[] fence_of_the_plot_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("FenceOfThePlot", websiteID);
                    if (announcement_Manssion.Fence_of_the_plot is null)
                        announcement_Manssion.Fence_of_the_plot = SearchFirstSynonymValue(nod, fence_of_the_plot_synonyms);

                    string[] shape_of_the_plot_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("ShapeOfThePlot", websiteID);
                    if (announcement_Manssion.Shape_of_the_plot is null)
                        announcement_Manssion.Shape_of_the_plot = SearchFirstSynonymValue(nod, shape_of_the_plot_synonyms);

                    string[] apperance_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Apperance", websiteID);
                    if (announcement_Manssion.Apperance is null)
                        announcement_Manssion.Apperance = SearchFirstSynonymValue(nod, apperance_synonyms);

                    string[] number_of_position_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("NumberOfPositions", websiteID);
                    if (announcement_Manssion.Number_of_positions is null)
                        announcement_Manssion.Number_of_positions = SearchFirstSynonymValue(nod, number_of_position_synonyms);

                    string[] building_material_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("BuildingMaterial", websiteID);
                    if(announcement_Manssion.Building_material is null)
                        announcement_Manssion.Building_material = SearchFirstSynonymValue(nod, building_material_synonyms);

                    string[] air_condition_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Air_conditioning", websiteID);
                    if (!announcement_Manssion.Air_conditioning)
                        announcement_Manssion.Air_conditioning = SearchFirstSynonymValue(nod, air_condition_synonyms) is null ? false : true;

                    string[] balcony_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Balcony", websiteID);
                    if (!announcement_Manssion.Balcony)
                        announcement_Manssion.Balcony = SearchFirstSynonymValue(nod, balcony_synonyms) is null ? false : true;

                    string[] basement_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Basement", websiteID);
                    if (!announcement_Manssion.Basement)
                        announcement_Manssion.Basement = SearchFirstSynonymValue(nod, basement_synonyms) is null ? false : true;

                    string[] garage_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Garage", websiteID);
                    if (!announcement_Manssion.Garage)
                        announcement_Manssion.Garage = SearchFirstSynonymValue(nod, garage_synonyms) is null ? false : true;

                    string[] garden_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Garden", websiteID);
                    if (!announcement_Manssion.Garden)
                        announcement_Manssion.Garden = SearchFirstSynonymValue(nod, garden_synonyms) is null ? false : true;

                    string[] lift_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Lift", websiteID);
                    if (!announcement_Manssion.Lift)
                        announcement_Manssion.Lift = SearchFirstSynonymValue(nod, lift_synonyms) is null ? false : true;

                    string[] non_smoking_only_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("NonSmokingOnly", websiteID);
                    if (!announcement_Manssion.Non_smoking_only)
                        announcement_Manssion.Non_smoking_only = SearchFirstSynonymValue(nod, non_smoking_only_synonyms) is null ? false : true;

                    string[] separate_kitchen_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("SeparateKitchen", websiteID);
                    if (!announcement_Manssion.Separate_kitchen)
                        announcement_Manssion.Separate_kitchen = SearchFirstSynonymValue(nod, separate_kitchen_synonyms) is null ? false : true;

                    string[] terrace_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Terrace", websiteID);
                    if (!announcement_Manssion.Terrace)
                        announcement_Manssion.Terrace = SearchFirstSynonymValue(nod, terrace_synonyms) is null ? false : true;

                    string[] two_storeys_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("TwoStoreys", websiteID);
                    if (!announcement_Manssion.Two_storeys)
                        announcement_Manssion.Two_storeys = SearchFirstSynonymValue(nod, two_storeys_synonyms) is null ? false : true;

                    string[] asphalt_access_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("AsphaltAccess", websiteID);
                    if (!announcement_Manssion.Asphalt_access)
                        announcement_Manssion.Asphalt_access = SearchFirstSynonymValue(nod, asphalt_access_synonyms) is null ? false : true;

                    string[] heating_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Heating", websiteID);
                    if (!announcement_Manssion.Heating)
                        announcement_Manssion.Heating = SearchFirstSynonymValue(nod, heating_synonyms) is null ? false : true;

                    string[] parking_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Parking", websiteID);
                    if (!announcement_Manssion.Parking)
                        announcement_Manssion.Parking = SearchFirstSynonymValue(nod, parking_synonyms) is null ? false : true;

                    string[] site_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Site", websiteID);
                    if (!announcement_Manssion.Site)
                        announcement_Manssion.Site = SearchFirstSynonymValue(nod, site_synonyms) is null ? false : true;

                    string[] type_of_roof_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("TypeOfRoof", websiteID);
                    if (announcement_Manssion.Type_of_roof is null)
                        announcement_Manssion.Type_of_roof = SearchFirstSynonymValue(nod, type_of_roof_synonyms);

                    string[] bungalow_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Bungalow", websiteID);
                    if (!announcement_Manssion.Bungalow)
                        announcement_Manssion.Bungalow = SearchFirstSynonymValue(nod, bungalow_synonyms) is null ? false : true;

                    string[] recreational_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Recreational", websiteID);
                    if (!announcement_Manssion.Recreational)
                        announcement_Manssion.Recreational = SearchFirstSynonymValue(nod, recreational_synonyms) is null ? false : true;

                    string[] investment_status_synonyms = new string[1];

                    string[] internet_synonyms = new string[1];

                    string[] cable_tv_synonyms = new string[1];

                    string[] phone_synonyms = _databaseService.GetAnnouncementsManssionSynonyms("Phone", websiteID);
                    if (!announcement_Manssion.Phone)
                        announcement_Manssion.Phone = SearchFirstSynonymValue(nod, phone_synonyms) is null ? false : true;

                    string[] preferences_synonyms = new string[1];

                    string[] market_synonyms = new string[1];
                }
            }
        }

        /// <summary>
        /// The function looks for synonyms in htmlnode, removes them and returns the processed data
        /// </summary>
        /// <param name="node">HtmlNoe</param>
        /// <param name="synonyms">Synonym array</param>
        /// <returns>Value from website</returns>
        private string? SearchFirstSynonymValue(HtmlNode node, string[] synonyms)
        {
            if (synonyms is null) return null;

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
            return null;
        }

        /// <summary>
        /// Start crawling full advertisment
        /// </summary>
        /// <param name="url">Page Url</param>
        private void StartAnnouncementCrawler(string url)
        {
            HtmlWeb web = new();
            Console.WriteLine(url);
            HtmlDocument doc = web.Load(url);
            HtmlNode[] nodes;
            Announcement? announcement = _databaseService.GetAnnouncementByLink(url);

            if (announcement is null)
                return;

            Console.WriteLine("DOKUMNET");
            foreach(var nod in doc.DocumentNode.SelectNodes("//div").Where(x => x.HasClass("//css-1q7h1ph")))
            {
                Console.WriteLine(nod.InnerText);
            }

            DownloadImagesUrl(announcement, doc);
            DownloadTitle(announcement, doc);
            DownloadPrice(announcement, doc);
            DownloadManssionProperties(announcement, doc);

            nodes = doc.DocumentNode.SelectNodes($"//{announcement.Crawler_Website.Crawler_Announcement.Description_node_name}")
                .Where(x => x.HasClass(announcement.Crawler_Website.Crawler_Announcement.Description_class_name)).ToArray();

            foreach( HtmlNode item in nodes)
                announcement.Description = item.InnerText.Trim();

            _databaseService.SaveChanges();
        }

        /// <summary>
        /// Checking if the webpage is in the database
        /// </summary>
        /// <param name="url">Url webpage</param>
        /// <returns>true or false</returns>
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
        /// A function that adds synonyms to the properties of a website
        /// </summary>
        /// <param name="announcement_Manssion_Synonyms">Announcement Manssion Synonyms array</param>
        public void AddSynonymsPropertiesToWebsite(Announcement_manssion_synonyms[] announcement_Manssion_Synonyms)
        {
            if(announcement_Manssion_Synonyms is null || announcement_Manssion_Synonyms.Length is 0)
                return;

            foreach(var synonym in announcement_Manssion_Synonyms)
            {
                if (synonym is not null)
                {
                    bool exists = _databaseService.CheckSynonymExists(synonym.Crawler_Website.Id, synonym.Announcement_dictionary_mansion_properties.Id, synonym.Value);
                    if (!exists)
                        _databaseService.AddSynonymPropertiesWebsite(synonym);
                }
            }
            _databaseService.SaveChanges();
        }

        /// <summary>
        /// A function that gets the id of the ad synonym
        /// </summary>
        /// <param name="property_name">Synonym name</param>
        /// <returns>Announcement_dictionary_mansion_properties</returns>
        public Announcement_dictionary_mansion_properties? GetAnnouncementMansionSynonymPropertiesId(string property_name) 
            => _databaseService.GetAnnonuncementDictionaryMansionPropertiesId(property_name);

        /// <summary>
        /// A function that gets the id of a website
        /// </summary>
        /// <param name="webName">Url Webpage</param>
        /// <returns>Crawler_website or null</returns>
        public Crawler_website? GetCrawlerAnnouncementId(string webName) => _databaseService.GetCrawlerAnnouncement(webName);

        /// <summary>
        /// A feature that adds a new website to the crawler
        /// </summary>
        /// <param name="website">Crawler website</param>
        public void AddWebsite(Crawler_website website)
        {
            if (website is null || WebsiteExists(website.Website)) return;

            _databaseService.AddCrawlerWebsite(website);
            _databaseService.SaveChanges();
        }

        /// <summary>
        /// The function that sets the status of sent advertisements
        /// </summary>
        /// <param name="announcement_Manssions">Announcement Manssions list</param>
        public void SetSendStatus(List<Announcement_manssion> announcement_Manssions)
        {
            foreach(Announcement_manssion am in announcement_Manssions)
                am.Announcement.Sent = true;

            _databaseService.SaveChanges();
        }

        /// <summary>
        /// Function show all webpages in cralwer database
        /// </summary>
        public void ShowWebpages()
        {
            List<Crawler_website> crawler_Websites = _databaseService.GetCrawlerWebsites();

            for(int i = 0; i < crawler_Websites.Count; i++)
                Console.WriteLine(crawler_Websites[i].Id + " " + crawler_Websites[i].Website + " " + crawler_Websites[i].Regex);
            Console.WriteLine("Pokazano strony");
        }

        /// <summary>
        /// Function stopping cralwer
        /// </summary>
        public void Stop()
        {
            IsWorking = false;
            _isRunning = false;
        }
    }
}
