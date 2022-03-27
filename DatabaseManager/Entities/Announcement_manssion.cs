using System.ComponentModel.DataAnnotations.Schema;

// TODO: Te wartości true/false trzeba przemyśleć, czy one mogą być null czy nie

namespace DatabaseManager.Entities
{
    // Encja do przechowywania danych typowo o ogłoszeniach związanych z nieruchomosciami
    public class Announcement_manssion
    {
        public int Id { get; set; }
        // Dopuszczona taka wartość jak kawalerka - dlatego string
        public string? Room_count { get; set; }

        //Dopuszczona wartość taka jak: parter/poddasze - dlatego string
        public string? Level { get; set; }

        //Czy umeblowane
        public bool? Furnished { get; set; }

        //Typ zabudowy
        public string? Type_of_building { get; set; }

        //Czynsz
        public int? Rent_price { get; set; }

        //Powierzchnia 
        //TODO: Pytanie czy nie lpiej bdzie string bo może być w różych jednostkach podana ta powierzchnia
        public int? Area { get; set; }

        // Rok budowy
        public int? Year_od_construction { get; set; }


        //Lokalizacja
        public string? Localization { get; set; }
        //TODO: Mam tu wpisane, żę powinno bć id sprzedawcy, ale to nie prawda, id sprzedawcy powinno być w innej encji


        // Głośność mieszkania
        public string? Volume { get; set; }

        // Dodatkowa powierzchnia - balkon, taras, komórka itp.
        public string? Additional_area { get; set; }

        // Typ zabudowy chyba już jest

        // Cena za metr kwadratowy
        public string? Price_per_m2 { get; set; }

        // Powierzchnia działki
        // TODO: Możliwe, że nie musi być to string
        public string? Land_area { get; set; }

        // Droga dojazdowa
        public string? Driveway { get; set; }

        // Stan mieszkania ( czy wyremontowane, do remontu itp. )
        public string? State { get; set; }

        // Ogrzewanie i energia
        public string? Heating_and_energy { get; set; }

        // Czy trzeba płacić za media itp.
        public string? Media { get; set; }

        // Ogrodzenie działki
        public string? Fence_of_the_plot { get; set; }

        //Kształt działki
        public string? Shape_of_the_plot { get; set; }

        // Wygląd (?)
        public string? Apperance { get; set; }

        // Liczba stanowisk (np. w garażu )
        public string? Number_of_positions { get; set; }

        // Materiał budynku
        public string? Building_material { get; set; }

        // Klimatyzacja
        public bool Air_conditioning { get; set; }

        // Balkon
        public bool Balcony { get; set; }

        // Piwnica
        public bool Basement { get; set; }
        // Garaż
        public bool Garage { get; set; }
        // Ogród
        public bool Garden { get; set; }
        // Winda
        public bool Lift { get; set; }
        // Tylko dla niepalących
        public bool Non_smoking_only { get; set; }
        // Oddzielna kuchnia
        public bool Separate_kitchen { get; set; }
        // Taras
        public bool Terrace { get; set; }
        // Dwie kondygnacje
        public bool Two_storeys { get; set; }
        // Pomieszczenie użytkowe
        public bool Utility_room { get; set; }
        // Dojazd asfaltowy
        public bool Asphalt_access { get; set; }
        // Ogrzewanie
        public bool Heating { get; set; }
        // Parking
        public bool Parking { get; set; }
        // Witryna
        public bool Site { get; set; }

        // Rodzaj dachu
        public string? Type_of_roof { get; set; }
        // Dom parterowy
        public bool Bungalow { get; set; }
        // Rekreacyjny
        public bool Recreational { get; set; }
        // Stan inwestycji
        public string? Investment_status { get; set; }
        // Internet
        public bool Internet { get; set; }
        // Telewizja kablowa
        public bool Cable_TV { get; set; }
        // Telefon
        public bool Phone { get; set; }

        // TODO: Może to trzeba zrobić jakoś inaczej, bo to w obecnej formie 
        // Przeznaczenie lokalu
        // Gastronomia

        // Przemysłowy
        // Hotelowy
        // Biurowy
        // Handlowy
        // Usługowy
        // Magazynowe
        // Produkcyjne

        // Preferencje ( kobiety/mężczyźni )
        public string? Preferences { get; set; }

        


        //Czy rynek wtórny czy nowy - dotyczyć powinno ofert sprzedaży mieszkań
        public string? Market { get; set; }

        //Klucz obcy co tabeli ogłoszeń
        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
    }
}
