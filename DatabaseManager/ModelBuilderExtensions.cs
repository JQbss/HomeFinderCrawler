using DatabaseManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement_dictionary_item>().HasData
                (
                    new Announcement_dictionary_item() { Id = 1, Name = "Mieszkanie" },
                    new Announcement_dictionary_item() { Id = 2, Name = "Dom" },
                    new Announcement_dictionary_item() { Id = 3, Name = "Garaż" },
                    new Announcement_dictionary_item() { Id = 4, Name = "Działka" }
                );

            modelBuilder.Entity<Announcements_dictionary_status>().HasData
                (
                    new Announcements_dictionary_status() { Id = 1, Name = "Aktywne" },
                    new Announcements_dictionary_status() { Id = 2, Name = "Usunięte" },
                    new Announcements_dictionary_status() { Id = 3, Name = "Wygaśnięte" }
                );

            modelBuilder.Entity<Announcement_dictionary_category>().HasData
                (
                    new Announcement_dictionary_category() { Id = 1, Name = "Sprzedaż" },
                    new Announcement_dictionary_category() { Id = 2, Name = "Wynajem" }
                );
            modelBuilder.Entity<Announcement_dictionary_mansion_properties>().HasData
                (
                    // Uzupełnienie słownika właściwości jakie mogą być pobierane ze stron
                    // Na podstawie których można później wyciągać synonimy
                    // Słownik jest po to, żeby wszystkie strony miały jednakową wartość właściwości
                    new Announcement_dictionary_mansion_properties() 
                    { 
                        Id = 1,
                        Name = "RoomCount",
                        Description = "Liczba pokoi w mieszkaniu"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 2,
                        Name = "Level",
                        Description = "Piętro na którym znajduje się mieszkanie"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 3,
                        Name = "Furnished",
                        Description = "Czy mieszkanie jest umeblowanie"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 4,
                        Name = "TypeOfBuild",
                        Description = "Typ budynku"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 5,
                        Name = "Area",
                        Description = "Miejsce"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 6,
                        Name = "YearOfConstruction",
                        Description = "Rok budowy mieszkania"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 7,
                        Name = "Location",
                        Description = "Lokacja nieruchomości"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 8,
                        Name = "Volume",
                        Description = "Głośność nieruchomości"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 9,
                        Name = "AdditionalArea",
                        Description = "Dodatkowa powierzchnia"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 10,
                        Name = "PricePerM2",
                        Description = "Cena za metr kwadratowy"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 11,
                        Name = "LandArea",
                        Description = "Powierzchnia działki"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 12,
                        Name = "Driveway",
                        Description = "Droga dojazdowa do posesji"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 13,
                        Name = "State",
                        Description = "Stan nieruchomości ( np. do remontu )"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 14,
                        Name = "HeatingAndEnergy",
                        Description = "Ogrzewanie i energia"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 15,
                        Name = "Media",
                        Description = "Media"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 16,
                        Name = "FenceOfThePlot",
                        Description = "Ogrodzenie działki"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 17,
                        Name = "ShapeOfThePlot",
                        Description = "Kształt ogrodzenia"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 18,
                        Name = "Apperance",
                        Description = "Wygląd zewnętrzny"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 19,
                        Name = "NumberOfPositions",
                        Description = "Liczba stanowisk w garażu"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 20,
                        Name = "BuildingMaterial",
                        Description = "Materiał budynku"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 21,
                        Name = "Air_conditioning",
                        Description = "Klimatyzacja"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 22,
                        Name = "Balcony",
                        Description = "Balkon"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 23,
                        Name = "Basement",
                        Description = "Piwnica"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 24,
                        Name = "Garage",
                        Description = "Garaż"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 25,
                        Name = "Garden",
                        Description = "Ogród"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 26,
                        Name = "Lift",
                        Description = "Winda"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 27,
                        Name = "NonSmokingOnly",
                        Description = "Tylko dla niepalących"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 28,
                        Name = "SeparateKitchen",
                        Description = "Oddzielna kuchnia"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 29,
                        Name = "Terrace",
                        Description = "Taras"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 30,
                        Name = "TwoStoreys",
                        Description = "Dwie kondygnacje"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 31,
                        Name = "UtilityRoom",
                        Description = "Pomieszczenie użytkowe"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 32,
                        Name = "AsphaltAccess",
                        Description = "Asfaltowy dojazd"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 33,
                        Name = "Heating",
                        Description = "Ogrzewanie"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 34,
                        Name = "Parking",
                        Description = "Parking"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 35,
                        Name = "Site",
                        Description = "Witryna"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 36,
                        Name = "TypeOfRoof",
                        Description = "Rodzaj dachu"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 37,
                        Name = "Bungalow",
                        Description = "Dom parterowy"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 38,
                        Name = "Recreational",
                        Description = "rekreacyjny"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 39,
                        Name = "InvestmentStatus",
                        Description = "Stan inwestycji"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 40,
                        Name = "Internet",
                        Description = "internet"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 41,
                        Name = "CableTV",
                        Description = "Telewizja kablowa"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 42,
                        Name = "Phone",
                        Description = "Telefon"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 43,
                        Name = "Preferences",
                        Description = "Preferencje"
                    },
                    new Announcement_dictionary_mansion_properties()
                    {
                        Id = 44,
                        Name = "Market",
                        Description = "Rynek (np. wtórny)"
                    }
                );
        }
    }
}
