namespace Rezerwacjabilet.Models
{
    public class UserReservation
    {
        public string Tytul { get; set; }
        public string Data { get; set; }
        public string Godzina { get; set; }
        public string[] Miejsca { get; set; }
    }

}
