using System.ComponentModel.DataAnnotations.Schema;

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

        //Czy rynek wtórny czy nowy - dotyczyć powinno ofert sprzedaży mieszkań
        public string? Market { get; set; }

        //Klucz obcy co tabeli ogłoszeń
        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
    }
}
