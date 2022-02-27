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
                    }
                );
        }
    }
}
