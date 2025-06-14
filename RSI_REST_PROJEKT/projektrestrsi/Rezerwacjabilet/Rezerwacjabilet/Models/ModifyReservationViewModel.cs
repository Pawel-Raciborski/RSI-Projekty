namespace Rezerwacjabilet.Models
{
    public class ModifyReservationViewModel
    {
        public string FilmId { get; set; }
        public string FilmTitle { get; set; }
        public string[] AvailableSeats { get; set; }
        public string[] ReservedSeats { get; set; }
        public string[] SelectedSeats { get; set; }
    }

}
